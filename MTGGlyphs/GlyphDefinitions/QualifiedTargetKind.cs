namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A target kind with its own qualifiers, in a list of choices where each kind is qualified separately: "red creature" in "target red creature or red planeswalker", "green enchantment" in "target forest, green enchantment, or green planeswalker".</summary>
/// <exampledoc>Chandra's Defeat</exampledoc>
/// <examplecapture>red planeswalker</examplecapture>
[Dependent]
public class QualifiedTargetKind : Glyph
{
    [Optional]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    public TargetKind Kind { get; set; }
}
