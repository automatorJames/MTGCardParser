namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An alternative kind joined by "or": the "or arcane" in "a spirit or arcane spell", "or sorcery" in "an instant or sorcery spell".</summary>
/// <exampledoc>Haru-Onna</exampledoc>
/// <examplecapture>or arcane</examplecapture>
[Dependent]
public class OrQualifier : Glyph
{
    public override Nib[] Nibs => ["or", Prop(Qualifier)];

    public TargetQualifier Qualifier { get; set; }
}
