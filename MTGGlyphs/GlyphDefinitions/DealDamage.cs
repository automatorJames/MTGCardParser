namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Dealing an amount of damage: "deals 1 damage to that player", "deals 4 damage to any target".</summary>
/// <exampledoc>Copper Tablet</exampledoc>
/// <examplecapture>deals 1 damage to that player</examplecapture>
[Dependent]
public class DealDamage : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("deals?"), Prop(Quantity), "damage to", Prop(Recipient)];

    public Quantity Quantity { get; set; }
    public Recipient Recipient { get; set; }
}
