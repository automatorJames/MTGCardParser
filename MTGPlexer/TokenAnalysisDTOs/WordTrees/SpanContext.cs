namespace MTGPlexer.TokenAnalysisDTOs.WordTrees;

public record SpanContext
{
    public string CardName { get; }
    public StructuredTokenRoot PrecedingToken { get; }
    public StructuredTokenRoot SpanToken { get; }
    public StructuredTokenRoot FollowingToken { get; }
    public string SpanText { get; }
    public string[] SpanWords { get; }
    public int SpanWordCount { get; }


    public SpanContext(string cardName, StructuredTokenRoot precedingToken, StructuredTokenRoot spanToken, StructuredTokenRoot followingToken)
    {
        CardName = cardName;
        PrecedingToken = precedingToken;
        SpanToken = spanToken;
        FollowingToken = followingToken;
        SpanText = spanToken.Value;
        SpanWords = SpanText.Split(' ');
        SpanWordCount = SpanWords.Length;
    }
    
}

