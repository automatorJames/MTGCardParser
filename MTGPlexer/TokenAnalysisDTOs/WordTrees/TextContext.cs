namespace MTGPlexer.TokenAnalysisDTOs.WordTrees;

public record TextContext
{
    public string CardName { get; }
    public TokenUnit PrecedingToken { get; }
    public TokenUnit AnchorToken { get; }
    public TokenUnit FollowingToken { get; }
    public string Text { get; }
    public string[] Words { get; }
    public int WordCount { get; }


    public TextContext(string cardName, TokenUnit precedingToken, TokenUnit token, TokenUnit followingToken)
    {
        CardName = cardName;
        PrecedingToken = precedingToken;
        AnchorToken = token;
        FollowingToken = followingToken;
        Text = token.Match.RootMatch.Value;
        Words = Text.Split(' ');
        WordCount = Words.Length;
    }
    
}

