namespace MTGGlyphs;

public class SacrificeIt : Glyph
{
    public override Nib[] Nibs => [Prop(Who), "sacrifice(s)? it"];

    public Who? Who { get; set; }
}