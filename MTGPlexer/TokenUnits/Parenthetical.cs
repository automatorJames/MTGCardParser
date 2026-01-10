namespace MTGPlexer.TokenUnits;

[NoSpaces]
[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
[TokenizationOrder(9999)]
[Color("#666666")]
public class Parenthetical : TokenUnit
{
    protected override string[] Snippets => [@"\(", nameof(Content), @"\)"];

    [RegexPattern(@"([^)]*)")]
    public PlaceholderCapture Content { get; set; }
}