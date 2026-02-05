namespace MTGPlexer.TokenUnits;

[NoSpaces]
[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
[TokenizationOrder(9999)]
[Color("#666666")]
public class Parenthetical : TokenUnit
{
    protected override Snippet[] Snippets => [@"\(", Prop(Content), @"\)"];

    [RegexPattern(@"([^)]*)")]
    public PrecursorCapture Content { get; set; }
}