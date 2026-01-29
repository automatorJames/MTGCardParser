
namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public record WrappedEnumNode : WrappedNode
{
    public WrappedEnumNode(Node parentNode, Type enumType, string name = null) : base(parentNode, name ?? enumType.Name, enumType)
    {
    }

    public override void ComposeRegexLines(RegexBuilder collector)
    {
        throw new NotImplementedException();
    }
}
