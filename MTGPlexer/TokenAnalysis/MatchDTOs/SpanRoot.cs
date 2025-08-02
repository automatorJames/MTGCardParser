namespace MTGPlexer.TokenAnalysis.MatchDTOs;
public record SpanRoot: SpanBranch
{
    public TokenUnit RootToken { get; }
    public TokenPlacement Placement { get; }

    public string AttachedPrecedingText { get; }
    public string AttachedFollowingText { get; set; }

    public SpanRoot(TokenUnit rootToken, string cardName, string precedingText = null) 
        : base(rootToken, cardName, parentPath: cardName, parentDepth: -1)
    {
        RootToken = rootToken;
        AttachedPrecedingText = precedingText;

        Placement = rootToken.Type
            .GetCustomAttribute<TokenPlacementAttribute>()?.Placement
            ?? TokenPlacement.Independent;
    }

    public override string ToString() => base.Text;
}