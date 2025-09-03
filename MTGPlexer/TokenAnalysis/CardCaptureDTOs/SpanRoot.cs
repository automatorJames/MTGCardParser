namespace MTGPlexer.TokenAnalysis.CardCaptureDTOs;

public record SpanRoot : SpanBranch
{
    public TokenUnit RootToken { get; }

    public SpanRoot(TokenUnit rootToken, string cardName, string originalLineText)
        : base(rootToken, cardName, parentPath: cardName, parentDepth: -1, originalLineText)
    {
        RootToken = rootToken;
    }

    public override string ToString() => base.Text;
}