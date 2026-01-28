
namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public record CollectionNode : ParentNode
{
    public Type ItemType { get; set; }

    public CollectionNode(Type itemType)
    {
        ItemType = itemType;
    }

    public override void ComposeRegexLines(RegexBuilder collector)
    {
        throw new NotImplementedException();
    }
}
