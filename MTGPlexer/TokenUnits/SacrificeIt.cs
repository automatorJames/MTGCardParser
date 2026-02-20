namespace MTGPlexer.TokenUnits;

public class SacrificeIt : TokenUnit
{
    public override Snippet[] Snippets => [Prop(Who), "sacrifice(s)? it"];

    public Who? Who { get; set; }
}