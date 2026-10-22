namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a red spell", "an artifact spell", "a spirit or arcane spell", "a red permanent spell", "a multicolored instant or sorcery spell", "a white or blue instant or sorcery spell", "a spell", "a creature spell with mana value 3 or less", "another blue spell": any one spell (or another one than this), optionally of colors or types (the shared comma/space-joined qualifier run), or bounded by a characteristic.</summary>
/// <exampledoc>Iron Star</exampledoc>
/// <examplecapture>a red spell</examplecapture>
[Dependent]
public class IndefiniteSpell : Glyph
{
    public override Nib[] Nibs => [Prop(Determiner), Prop(Qualifiers), "spell", Prop(With)];

    public IndefiniteDeterminer Determiner { get; set; }
    [Optional]
    public CompoundOf<QualifierWords> Qualifiers { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
