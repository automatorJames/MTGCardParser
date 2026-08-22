namespace MTGGlyphs.GlyphDefinitions;

public class LifeQuantity : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), "life"];

    public Quantity Quantity { get; set; }
}