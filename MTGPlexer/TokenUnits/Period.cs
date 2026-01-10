namespace MTGPlexer.TokenUnits;

[TokenizationOrder(-1)]
[Color("#999999")]
public class Period : TokenUnit
{
    protected override Snippet[] Snippets => [@"\."];    
}

