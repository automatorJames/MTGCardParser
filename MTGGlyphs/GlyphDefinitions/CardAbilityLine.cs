namespace MTGGlyphs.GlyphDefinitions;

public class CardAbilityLine : Glyph
{
    public override Nib[] Nibs => ["^", Prop(Keyword), "$"];
    public override Joiner Joiner => Joiner.CommaSpace;
    public Keyword Keyword { get; set; }
}