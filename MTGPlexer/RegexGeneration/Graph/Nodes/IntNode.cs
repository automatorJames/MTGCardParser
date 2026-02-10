namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class IntNode : ScalarContainerNode
{
    string[] _captureAlternatives;

    public IntNode(RegexNode parentNode, PropNavigation navigation) 
        : base(parentNode, navigation)
    {
        if (navigation.RegexPatterns == null || navigation.RegexPatterns.Length == 0)
            throw new Exception($"Int properties are required to define at least one RegexPattern");
    }

    protected override List<RegexNode> GetChildNodes() =>
        _captureAlternatives
        .Select((x, idx) => new ScalarNode(this, scalarValue: true, name: x, isFirst: idx == 0))
        .Cast<RegexNode>()
        .ToList();

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(GroupOpenBrick);
        collector.AppendJoined(Children, GetJoinerBrick(Joiner.Pipe));
        collector.Append(GroupCloseBrick);
    }
}

    //public override object GetValueSingle(Capture capture)
    //{
    //    // Simply return "true", because TerminalNode already validated that the
    //    // named group exists, and therefore this bool check has already succeeded
    //
    //    return true;
    //}
}