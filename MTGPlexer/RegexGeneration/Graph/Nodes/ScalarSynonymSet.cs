namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class ScalarSynonymSet : RegexNode, INamedScalarValue
{
    List<ScalarNode> _scalarChildren = [];

    public object ScalarValue { get; }
    public string[] ScalarSynonyms { get; }
    public Regex Regex { get; }


    public ScalarSynonymSet(
        ScalarContainerNode parentNode, 
        string name,
        object scalarValue,
        IEnumerable<string> scalarSynonyms, 
        int positionAmongSiblings) 
        : base(parentNode, name)
    {
        ScalarValue = scalarValue;
        ScalarSynonyms = scalarSynonyms.ToArray();

        _scalarChildren = scalarSynonyms.Select((x, idx) => new ScalarNode(
                parentNode: this,
                name: x.Replace(' ', '-'), // no spaces allowed for FullyQualifiedName purposes
                scalarValue: ScalarValue,
                regex: x,
                positionAmongSiblings: positionAmongSiblings,
                positionAmongSynonyms: idx))
            .ToList();

        Regex = new(string.Join('|', _scalarChildren.Select(x => x.RegexString)));
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        for (int i = 0; i < _scalarChildren.Count; i++)
        {
            _scalarChildren[i].AppendRegexBricks(collector);
    
            if (i < _scalarChildren.Count - 1)
                collector.Append(new RegexBrickJoiner(this, Joiner.Pipe));
        }
    }
}
