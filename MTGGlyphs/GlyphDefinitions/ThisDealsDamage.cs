namespace MTGGlyphs.GlyphDefinitions;

public class ThisDealsDamage : Glyph
{
    public override Nib[] Nibs => ["{this} deals", Prop(Quantity), "damage to", Prop(Recipient)];

    public Quantity Quantity { get; set; }
    public Recipient Recipient { get; set; }
}
