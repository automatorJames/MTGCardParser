namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a property on a Glyph whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public class EnumNode : NamedGroupNode
{
    protected override Joiner? ChildJoiner => Joiner.Pipe;

    public EnumNode(RegexNode parentNode, Navigation navigation) : base(parentNode, navigation)
    {
    }

    /// <summary>Adds one <see cref="EnumMemberNode"/> per (member, synonym pattern) pair — including plural variants when <see cref="OptionalPluralAttribute"/> is declared on the enum type.</summary>
    protected override void AddReflectedChildren(List<RegexNode> children)
    {
        var enumType = Navigation.UnderlyingType;
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
                children.Add(new EnumMemberNode(
                    parentNode: this,
                    name: enumAsString,
                    scalarValue: enumMember,
                    regexString: patterns[j],
                    positionAmongSiblings: i,
                    positionAmongSynonyms: enumMembers.Count > 1 ? j : null));
        }
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

        base.AppendInnerContentBricks(collector);

        if (carriesJoiner)
            collector.Append(new RegexBrick(this, ")"));
    }

    /// <summary>Finds which <see cref="EnumMemberNode"/> child's pattern matched the captured text, and returns its scalar enum value.</summary>
    protected override object GetValue(CaptureTrace captureTrace)
    {
        if (captureTrace.Count != 1)
            throw new Exception($"{nameof(EnumNode)} expects exactly one capture");

        // Without the joiners an optional enum carries inside its group (see AppendInnerContentBricks): the members'
        // patterns are anchored, and match the member alone.
        var value = captureTrace.CaptureValue.Trim(' ');

        return Children
            .OfType<EnumMemberNode>()
            .FirstOrDefault(x => x.Regex.IsMatch(value))
            ?.ScalarValue
            ?? throw new Exception($"Found no matching values for enum '{Navigation.UnderlyingType.Name}' from match string '{captureTrace.CaptureValue}'");
    }
}
