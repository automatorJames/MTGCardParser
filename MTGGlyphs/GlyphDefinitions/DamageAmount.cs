namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A counted amount of damage: "3 damage".</summary>
/// <exampledoc>Psionic Blast</exampledoc>
/// <examplecapture>4 damage</examplecapture>
[Dependent]
public class DamageAmount : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), "damage"];

    public Quantity Quantity { get; set; }
}
