namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>A leaf node wrapping one literal regex pattern string, with no children of its own.</summary>
public class TerminalRegexNode : RegexNode
{
    /// <summary>The literal regex pattern this node contributes.</summary>
    public string RegexString { get; }

    public TerminalRegexNode(
        RegexNode parentNode,
        string name,
        string regexString)
        : base(parentNode, name)
    {
        RegexString = regexString;
    }

    protected override void AppendOwnRegexBricks(RegexCollector collector) =>
        collector.Append(new RegexBrick(this, RegexString));
}