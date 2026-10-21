namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// One (enum member, synonym pattern) pair within an <see cref="EnumNode"/>'s children — a member with N declared synonym
/// patterns produces N of these. Built on request from the enum's shared <see cref="EnumNode.Members"/>, and not kept
/// between requests (see <see cref="EnumNode"/>).
/// </summary>
public class EnumMemberNode : TerminalRegexNode
{
    /// <summary>The enum member value this node's pattern matches.</summary>
    public object ScalarValue { get; }

    /// <summary>A compiled regex for <see cref="TerminalRegexNode.RegexString"/>, used to test which member matched.</summary>
    public Regex Regex { get; }

    /// <summary>Index among this member's sibling members within the enum (not among its own synonym patterns).</summary>
    public int PositionAmongSiblings { get; }

    /// <summary>Index among this member's own synonym patterns, or null if the member has only one pattern.</summary>
    public int? PositionAmongSynonyms { get; }

    public EnumMemberNode(
        RegexNode parentNode,
        string name,
        object scalarValue,
        string regexString,
        int positionAmongSiblings,
        int? positionAmongSynonyms)
        : base(parentNode, name, regexString)
    {
        ScalarValue = scalarValue;
        Regex = AnchoredRegexFor(regexString);
        PositionAmongSiblings = positionAmongSiblings;
        PositionAmongSynonyms = positionAmongSynonyms;
    }

    /// <summary>
    /// <paramref name="pattern"/> anchored, so it tells whether a captured value *is* the member: a bare pattern matches
    /// anywhere within the value, so e.g. "fish" would claim a captured "fish sticks", and "cat" a captured "catfish",
    /// whenever that member happens to be declared first. Shared by pattern: the same few patterns recur across every
    /// enum of every graph.
    /// </summary>
    internal static Regex AnchoredRegexFor(string pattern) =>
        _anchoredRegexes.GetOrAdd(pattern, x => new($"^(?:{x})$"));

    static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Regex> _anchoredRegexes = new();

    /// <summary>Appends this member's value brick - what its own bricks are (see <see cref="EnumNode.AppendInnerContentBricks"/>, which emits each member's).</summary>
    internal void AppendValueBrick(RegexCollector collector) =>
        collector.Append(new RegexBrickValue(this, RegexString, ScalarValue));

    protected override void AppendOwnRegexBricks(RegexCollector collector) =>
        AppendValueBrick(collector);
}
