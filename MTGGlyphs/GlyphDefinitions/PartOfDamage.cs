namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"1 of", "x of": how much of the damage just named is prevented.</summary>
/// <exampledoc>Guardian Seraph</exampledoc>
/// <examplecapture>1 of</examplecapture>
[Dependent]
public class PartOfDamage : Glyph
{
    public override Nib[] Nibs => [Prop(Amount), "of"];

    public Quantity Amount { get; set; }
}
