namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"{this} deals 1 damage to that player": the card itself dealing an amount of damage.</summary>
/// <exampledoc>Copper Tablet</exampledoc>
/// <examplecapture>{this} deals 1 damage to that player</examplecapture>
public class ThisDealsDamage : Glyph
{
    public override Nib[] Nibs => ["{this} deals", Prop(Quantity), "damage to", Prop(Recipient)];

    public Quantity Quantity { get; set; }
    public Recipient Recipient { get; set; }
}
