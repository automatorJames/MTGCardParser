namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>One (enum member, synonym pattern) pair within an <see cref="EnumNode"/>'s children — a member with N declared synonym patterns produces N of these.</summary>
public class EnumMemberNode : TerminalRegexNode
{
    /// <summary>The enum member value this node's pattern matches.</summary>
    public object ScalarValue { get; }

    /// <summary>A compiled regex for <see cref="TerminalRegexNode.RegexString"/>, used by <see cref="EnumNode.GetValue"/> to test which member matched.</summary>
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
        Regex = new (regexString);
        PositionAmongSiblings = positionAmongSiblings;
        PositionAmongSynonyms = positionAmongSynonyms;
    }

    public override void AppendRegexBricks(RegexCollector collector) =>
        collector.Append(new RegexBrickValue(this, RegexString, ScalarValue));
}
