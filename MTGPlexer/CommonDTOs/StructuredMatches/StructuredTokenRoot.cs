namespace MTGPlexer.CommonDTOs.StructuredMatches;

public record StructuredTokenRoot : StructuredMatchBase
{

    public Type TokenType { get; set; }
    public override int AbsoluteStartInSource { get; }
    public override int AbsoluteEndInSource { get; }
    public override string SourceText { get; }

    public StructuredTokenRoot(Type tokenType, string sourceText, int absoluteStartInSource, int absoluteEndInSource) 
        : base(GetMatch(tokenType, sourceText, absoluteStartInSource, absoluteEndInSource))
    {
        TokenType = tokenType;
        AbsoluteStartInSource = absoluteStartInSource;
        AbsoluteEndInSource = absoluteEndInSource;
        SourceText = sourceText;
    }

    //static Match GetMatch(Type tokenType, string sourceText) => tokenType == typeof(DefaultUnmatchedString) ?
    //        _matchAll.Match(sourceText)
    //        : TokenTypeRegistry.Templates[tokenType].Regex.Match(sourceText);

    static Match GetMatch(Type tokenType, string sourceText, int absoluteStartInSource, int absoluteEndInSource) => tokenType == typeof(DefaultUnmatchedString) ?
        Regex.Match(sourceText, sourceText.Substring(absoluteStartInSource, absoluteEndInSource - absoluteStartInSource))
        : TokenTypeRegistry.Templates[tokenType].Regex.Match(sourceText);
}