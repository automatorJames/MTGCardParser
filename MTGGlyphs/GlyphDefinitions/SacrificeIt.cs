namespace MTGGlyphs.GlyphDefinitions;

public class SacrificeIt : Glyph
{
    public override Nib[] Nibs => [Prop(Who), Pattern("sacrifice(s)?"), "it"];

    public Who? Who { get; set; }
}