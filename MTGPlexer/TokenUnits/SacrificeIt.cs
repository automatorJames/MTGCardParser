namespace MTGPlexer.TokenUnits;

public class SacrificeIt : TokenUnit
{
    protected override Snippet[] Snippets => [Prop(Who), "sacrifice(s)? it"];

    public Who? Who { get; set; }
}