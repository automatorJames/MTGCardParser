namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class Parenthetical : TokenUnitBase
{
    public RegexTemplate<Parenthetical> RegexTemplate => new(@"\(", nameof(Content), @"\)");

    [RegexPattern(@"([^)]*)")]
    public CapturedTextSegment Content { get; set; }
}