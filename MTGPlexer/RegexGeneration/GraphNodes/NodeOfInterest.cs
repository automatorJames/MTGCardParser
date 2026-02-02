
namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class NodeOfInterest : Node
{
    protected NodeOfInterest(Node parentNode, string name) : base(parentNode, name)
    {
    }
}
