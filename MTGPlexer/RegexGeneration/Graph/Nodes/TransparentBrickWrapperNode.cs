
namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class TransparentBrickWrapperNode : RegexNode
{
    RegexBrick _brick;

    public TransparentBrickWrapperNode(RegexNode parentNode, string name, RegexBrick brick) 
        : base(parentNode, name)
    {
        _brick = brick;
    }

    public override void AppendRegexBricks(RegexCollector collector) =>
        collector.Append(_brick);
}