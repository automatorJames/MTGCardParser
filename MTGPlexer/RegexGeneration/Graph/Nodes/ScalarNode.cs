namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class ScalarNode : RegexNode, INamedScalarValue
{
    public object ScalarValue { get; }
    public string Regex { get; }
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
        Regex = regex;
        PositionAmongSiblings = positionAmongSiblings;
        PositionAmongSynonyms = positionAmongSynonyms;
    }

    public override void AppendRegexBricks(RegexCollector collector) =>
        collector.Append(new RegexBrickTerminal(this, Regex, null, ScalarValue, PositionAmongSiblings, PositionAmongSynonyms));
}
