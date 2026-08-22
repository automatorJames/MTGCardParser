namespace MTGGlyphs.GlyphDefinitions;

public class LifeChangeQuantity : Glyph
{
    public override Nib[] Nibs => [Prop(WhichPlayer), Prop(LifeVerb), Prop(Quantity), "life"];

    public WhichPlayer WhichPlayer { get; set; }
    public LifeVerb LifeVerb { get; set; }
    public Quantity Quantity { get; set; }
}

public enum LifeVerb
{
    Gain,
    Lose
}