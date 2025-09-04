namespace MTGPlexer.TokenUnits;

[IgnoreInAnalysis]
[TokenizationOrder(9999)]
[Color("#999999")]
public class PunctuationEnclosing : TokenUnit
{
    public EnclosingPunctuationCharacter EnclosingPunctuationCharacter { get; set; }
}

[RegexEnum]
public enum EnclosingPunctuationCharacter
{
    [RegexPattern(@"""")]
    DoubleQuote,
}