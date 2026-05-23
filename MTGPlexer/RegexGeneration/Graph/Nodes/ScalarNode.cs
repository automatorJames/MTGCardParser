namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class ScalarNode : RegexNode, INamedScalarValue
{
    public object ScalarValue { get; }
    public string RegexString { get; }
    public Regex Regex { get; }
    public int PositionAmongSiblings { get; }
    public int PositionAmongSynonyms { get; }

    public ScalarNode(
        RegexNode parentNode, 
        string name,
        object scalarValue,
        string regex,
        int positionAmongSiblings,
        int positionAmongSynonyms = 0) 
        : base(parentNode, name)
    {
        ScalarValue = scalarValue;
        RegexString = regex;
        Regex = new (RegexString);
        PositionAmongSiblings = positionAmongSiblings;
        PositionAmongSynonyms = positionAmongSynonyms;
    }

    public override void AppendRegexBricks(RegexCollector collector) =>
        collector.Append(new RegexBrickTerminal(this, RegexString, null, ScalarValue, PositionAmongSiblings, PositionAmongSynonyms));
}
