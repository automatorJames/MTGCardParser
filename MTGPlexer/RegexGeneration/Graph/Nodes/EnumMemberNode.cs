namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class EnumMemberNode : TerminalRegexNode
{
    public object ScalarValue { get; }
    public Regex Regex { get; }
    public int PositionAmongSiblings { get; }
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
        collector.Append(new RegexBrickValue(this, RegexString, null, ScalarValue));
}
