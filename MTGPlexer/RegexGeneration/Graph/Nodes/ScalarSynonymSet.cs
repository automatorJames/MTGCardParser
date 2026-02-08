namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class ScalarSynonymSet : BranchNode, INamedScalarValue
{
    public object ScalarValue { get; }
    string[] _scalarSynonyms;
    bool _isFirst;

    public ScalarSynonymSet(
        ScalarContainerNode parentNode, 
        TypeNavigation typeNavigation, 
        object scalarValue,
        string[] scalarSynonyms, 
        bool isFirst = false) 
        : base(parentNode, typeNavigation)
    {
        ScalarValue = typeNavigation.Name; // "Name" is expected to be a representative value like an Enum member ToString()
        _scalarSynonyms = scalarSynonyms;
        _isFirst = isFirst;
    }

    protected override List<RegexNode> GetChildNodes()
    {
        return
            _scalarSynonyms
            .Select((x, idx) => (RegexNode)new ScalarNode(this, ScalarValue, x, isFirst: _isFirst && idx == 0, isSynonym: idx > 0))
            .ToList();
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        Children.ForEach(x => x.AppendRegexBricks(collector));
    }
}
