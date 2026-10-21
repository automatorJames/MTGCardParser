using System.Collections.Concurrent;

namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a property on a Glyph whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// <para>
/// A vocabulary is inlined into every graph that uses it, once per use - a large grammar's graphs would hold millions of
/// member nodes. So what the members are is kept once per enum type (see <see cref="Members"/>), and the
/// <see cref="EnumMemberNode"/> children are built on request and not kept (see <see cref="RetainChildren"/>): the
/// engine never needs them, only the bricks they produce, which are built from the shared members directly.
/// </para>
/// </summary>
public class EnumNode : NamedGroupNode
{
    protected override Joiner? ChildJoiner => Joiner.Pipe;

    public EnumNode(RegexNode parentNode, Navigation navigation) : base(parentNode, navigation)
    {
    }

    /// <summary>One (member, synonym pattern) pair of an enum type - what one <see cref="EnumMemberNode"/> stands for.</summary>
    /// <param name="Regex"><paramref name="Pattern"/> anchored, for telling which member a captured value is (see <see cref="EnumMemberNode.Regex"/>).</param>
    public sealed record Member(string Name, object Value, string Pattern, Regex Regex, int PositionAmongSiblings, int? PositionAmongSynonyms);

    static readonly ConcurrentDictionary<Type, IReadOnlyList<Member>> _membersByEnumType = new();

    /// <summary>The enum's members, one per (member, synonym pattern) pair - including plural variants when <see cref="OptionalPluralAttribute"/> is declared on the enum type - shared by every node of the enum type.</summary>
    public IReadOnlyList<Member> Members => _membersByEnumType.GetOrAdd(Navigation.UnderlyingType, ReadMembers);

    static IReadOnlyList<Member> ReadMembers(Type enumType)
    {
        var members = new List<Member>();
        var enumMembers = Enum.GetValues(enumType).Cast<object>().ToList();

        for (int i = 0; i < enumMembers.Count; i++)
        {
            var enumMember = enumMembers[i];
            var enumAsString = enumMember.ToString();
            var field = enumType.GetField(enumAsString);

            List<string> patterns =
                field.GetCustomAttribute<RegexPatternAttribute>()?.Patterns.ToList()
                ?? [enumAsString.ToFriendlyCase(TitleDisplayOption.Lower)];

            if (enumType.IsDefined(typeof(OptionalPluralAttribute)))
            {
                patterns = patterns
                    .SelectMany(x => new[] { x, x.AddPluralization(makeOptional: false) })
                    .ToList();
            }

            for (int j = 0; j < patterns.Count; j++)
                members.Add(new(enumAsString, enumMember, patterns[j], EnumMemberNode.AnchoredRegexFor(patterns[j]), i, enumMembers.Count > 1 ? j : null));
        }

        return members;
    }

    /// <summary>Built on each request, not kept: see the class remarks.</summary>
    protected override bool RetainChildren => false;

    /// <summary>An enum's children are its members, none of them a named group.</summary>
    public override List<NamedGroupNode> NamedGroupChildren => [];

    /// <summary>Adds one <see cref="EnumMemberNode"/> per <see cref="Members"/> entry.</summary>
    protected override void AddReflectedChildren(List<RegexNode> children)
    {
        foreach (var member in Members)
            children.Add(new EnumMemberNode(this, member.Name, member.Value, member.Pattern, member.PositionAmongSiblings, member.PositionAmongSynonyms));
    }

    /// <summary>
    /// The members, as alternatives - grouped when the enum carries a joiner inside its own group (as an optional
    /// enum does - see <see cref="NamedGroupNode.AppendOwnRegexBricks"/>), or the joiner would precede the first member
    /// alone: <c>( fish|milk)?</c> matches " fish" and a bare "milk", where <c>( (fish|milk))?</c> is meant.
    /// </summary>
    protected override void AppendInnerContentBricks(RegexCollector collector)
    {
        var carriesJoiner = LeadingJoinerPlacement == JoinerPlacement.InsideNodeLeading || TrailingJoinerSuccessor is not null;

        if (carriesJoiner)
            collector.Append(new RegexBrick(this, "("));

        // One list of member nodes for this walk. Each is emitted here, not through RegexNode.AppendRegexBricks, which
        // would look the member up among a fresh list of children (see RetainChildren) - and would place its leading
        // joiner by the general rules, which for a member come to just this: the enum's pipe before every member but
        // the first, unless the regex so far already ends in a space (see JoinerRules.Between).
        var members = Children;

        for (int i = 0; i < members.Count; i++)
        {
            if (i > 0 && !collector.AlreadySeparated)
                collector.Append(new RegexBrickJoiner(this, Joiner.Pipe));

            ((EnumMemberNode)members[i]).AppendValueBrick(collector);
        }

        if (carriesJoiner)
            collector.Append(new RegexBrick(this, ")"));
    }

    /// <summary>Finds which member's pattern matched the captured text, and returns its scalar enum value.</summary>
    protected override object GetValue(CaptureTrace captureTrace)
    {
        if (captureTrace.Count != 1)
            throw new Exception($"{nameof(EnumNode)} expects exactly one capture");

        // Without the joiners an optional enum carries inside its group (see AppendInnerContentBricks): the members'
        // patterns are anchored, and match the member alone.
        var value = captureTrace.CaptureValue.Trim(' ');

        foreach (var member in Members)
            if (member.Regex.IsMatch(value))
                return member.Value;

        throw new Exception($"Found no matching values for enum '{Navigation.UnderlyingType.Name}' from match string '{captureTrace.CaptureValue}'");
    }
}
