namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spells of a kind, with no quantifier: "spells with the same name as this spell from among those cards" (ripple), "noncreature spells from among those cards", "spells from among cards exiled with this enchantment", or with no source in free casts ("cast dragon spells without paying their mana costs"). PlayCard needs "without paying", so this doesn't compete with PlayFromZone's "spells from your graveyard".</summary>
/// <exampledoc>Surging Aether</exampledoc>
/// <examplecapture>spells with the same name as this spell from among those cards</examplecapture>
[Dependent]
public class SpellsOfKind : Glyph
{
    public override Nib[] Nibs => [Prop(Kind), Prop(With), Prop(From)];

    public OneOf<QualifiedSpellsNoun, SpellsNoun> Kind { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
    [Optional]
    public FromAmongCards? From { get; set; }
}
