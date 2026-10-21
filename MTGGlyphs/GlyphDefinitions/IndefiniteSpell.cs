namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a red spell", "an artifact spell", "a spirit or arcane spell", "a red permanent spell", "a multicolored instant or sorcery spell", "a spell", "a creature spell with mana value 3 or less": any one spell, optionally of a color or type (a second, space-joined qualifier narrows further: "red permanent"), or bounded by a characteristic.</summary>
/// <exampledoc>Iron Star</exampledoc>
/// <examplecapture>a red spell</examplecapture>
[Dependent]
public class IndefiniteSpell : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Qualifiers), Prop(Second), Prop(Or), "spell", Prop(With)];

    [Optional]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    [Optional]
    public TargetQualifier Second { get; set; }
    [Optional]
    public OrQualifier Or { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
