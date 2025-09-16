namespace MTGPlexer.TokenAnalysisDTOs.WordTrees;

public record SpanContext
{
    public string CardName { get; }
    public TokenUnit PrecedingToken { get; }
    public TokenUnit SpanToken { get; }
    public TokenUnit FollowingToken { get; }
    public string SpanText { get; }
    public string[] SpanWords { get; }
    public int SpanWordCount { get; }


    public SpanContext(string cardName, TokenUnit precedingToken, TokenUnit spanToken, TokenUnit followingToken)
    {
        CardName = cardName;
        PrecedingToken = precedingToken;
        SpanToken = spanToken;
        FollowingToken = followingToken;
        SpanText = spanToken.Capture.Value;
        SpanWords = SpanText.Split(' ');
        SpanWordCount = SpanWords.Length;
    }
    
}

