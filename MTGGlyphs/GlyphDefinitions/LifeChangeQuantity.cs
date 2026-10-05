namespace MTGGlyphs.GlyphDefinitions;

public class LifeChangeQuantity : Glyph
{
    public override Nib[] Nibs => [Prop(WhichPlayer), Prop(GainOrLose), Prop(Quantity), "life"];

    public WhichPlayer WhichPlayer { get; set; }
    public GainOrLose GainOrLose { get; set; }
    public Quantity Quantity { get; set; }
}
