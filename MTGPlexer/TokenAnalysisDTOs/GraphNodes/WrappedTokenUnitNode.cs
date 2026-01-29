
namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public record WrappedTokenUnitNode : WrappedNode
{
    public WrappedTokenUnitNode(Node parentNode, Type type, string name = null) : base(parentNode, name ?? type.Name, type)
    {
    }

    public override void ComposeRegexLines(RegexBuilder collector)
    {
        throw new NotImplementedException();
    }
}
