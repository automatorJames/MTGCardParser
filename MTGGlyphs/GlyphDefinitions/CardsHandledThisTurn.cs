namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spells or cards a player has cast, drawn or discarded this turn, as something counted: "spell you've cast this turn", "cards you've discarded this turn", "other spells you've cast this turn", "noncreature spells they've cast this turn", "cards that player has drawn this turn", or with no player named ("other spell cast this turn"). "other" and the qualifiers share one optional slot: two adjacent optional slots stop the match. Used by "for each …" and "the number of …".</summary>
/// <exampledoc>Aetherflux Reservoir</exampledoc>
/// <examplecapture>spell you've cast this turn</examplecapture>
[Dependent]
public class CardsHandledThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Kinds), Plural(Prop(Noun)), Prop(Player), Prop(Verb), "this turn"];

    [Optional]
    public OneOf<OtherKinds, QualifierWords> Kinds { get; set; }
    public SpellOrCard Noun { get; set; }
    [Optional]
    public OneOf<PerfectSubject?, WhichPlayer?, PlayerHas> Player { get; set; }
    public CardHistoryVerb Verb { get; set; }
}
