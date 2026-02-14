namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class ScalarSynonymSet : RegexNode, INamedScalarValue
{
    public object ScalarValue { get; }

    bool _isFirst;
    List<ScalarNode> _scalarChildren = [];

    public ScalarSynonymSet(
        ScalarContainerNode parentNode, 
        string name,
        object scalarValue,
        string[] scalarSynonyms, 
        bool isFirst = false) 
        : base(parentNode, name)
    {
        ScalarValue = scalarValue;
        _isFirst = isFirst;

        _scalarChildren = scalarSynonyms.Select((x, idx) => new ScalarNode(
                parentNode: this,
                name: $"Synonym-{idx}",
                scalarValue: ScalarValue,
                regex: x,
                isFirst: _isFirst && idx == 0,
                isSecondarySynonym: idx > 0))
            .ToList();
    }

    public override void AppendRegexBricks(RegexCollector collector) =>
        collector.AppendJoinedAlternating(this, _scalarChildren.Cast<RegexNode>().ToList());
}
