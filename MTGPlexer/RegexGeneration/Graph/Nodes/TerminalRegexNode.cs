namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class TerminalRegexNode : RegexNode
{
    public string RegexString { get; }

    public TerminalRegexNode(
        RegexNode parentNode,
        string name,
        string regexString)
        : base(parentNode, name)
    {
        RegexString = regexString;
    }

    public override void AppendRegexBricks(RegexCollector collector) =>
        collector.Append(new RegexBrick(this, RegexString, null));
}