namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a red spell", "an artifact spell", "a spell": any one spell, optionally of a color or type.</summary>
/// <exampledoc>Iron Star</exampledoc>
/// <examplecapture>a red spell</examplecapture>
[Dependent]
public class IndefiniteSpell : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Qualifiers), "spell"];

    [Optional]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
}
