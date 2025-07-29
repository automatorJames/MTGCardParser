namespace MTGPlexer.TokenAnalysis.DTOs;

public record SpanContext
{
    public string CardName { get; }
    public Token<Type>? PrecedingToken { get; }
    public Token<Type> SpanToken { get; }
    public Token<Type>? FollowingToken { get; }
    public string SpanText { get; }
    public string[] SpanWords { get; }
    public int SpanWordCount { get; }


    public SpanContext(string cardName, Token<Type>? precedingToken, Token<Type> spanToken, Token<Type>? followingToken)
    {
        CardName = cardName;
        PrecedingToken = precedingToken;
        SpanToken = spanToken;
        FollowingToken = followingToken;
        SpanText = spanToken.ToStringValue();
        SpanWords = SpanText.Split(' ');
        SpanWordCount = SpanWords.Length;
    }
    
}

