namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"1 of", "x of", "all but 1 of": how much of the damage just named is prevented.</summary>
/// <exampledoc>Guardian Seraph</exampledoc>
/// <examplecapture>1 of</examplecapture>
[Dependent]
public class PartOfDamage : Glyph
{
    public override Nib[] Nibs => [Prop(AllBut), Prop(Amount), "of"];

    [RegexPattern("all but")]
    public bool AllBut { get; set; }
    public Quantity Amount { get; set; }
}
