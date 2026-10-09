namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the next 3": a fixed amount of damage to prevent.</summary>
/// <exampledoc>Healing Salve</exampledoc>
/// <examplecapture>the next 3</examplecapture>
[Dependent]
public class NextDamage : Glyph
{
    public override Nib[] Nibs => ["the next", Prop(Quantity)];

    public Quantity Quantity { get; set; }
}
