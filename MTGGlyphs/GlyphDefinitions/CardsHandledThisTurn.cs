namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spells or cards a player has cast, drawn or discarded this turn, as something counted: "spell you've cast this turn", "cards you've discarded this turn", "other spells you've cast this turn", "noncreature spells they've cast this turn", "cards that player has drawn this turn". Used by "for each …" and "the number of …".</summary>
/// <exampledoc>Aetherflux Reservoir</exampledoc>
/// <examplecapture>spell you've cast this turn</examplecapture>
[Dependent]
public class CardsHandledThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Other), Prop(Qualifiers), Plural(Prop(Noun)), Prop(Player), Opt("has"), Prop(Verb), "this turn"];

    [Optional]
    [RegexPattern("other")]
    public bool? Other { get; set; }
    [Optional]
    public QualifierWords Qualifiers { get; set; }
    public SpellOrCard Noun { get; set; }
    public OneOf<PerfectSubject?, WhichPlayer?, PlayerReference> Player { get; set; }
    public CardHistoryVerb Verb { get; set; }
}
