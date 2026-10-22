namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards in a player's zone, to count, optionally of a type or a choice of types, sharing the card's name, or narrowed by a "with" qualifier: "cards in their hand", "creature cards in your graveyard", "(each) card in your hand", "instant and sorcery cards in your graveyard", "instant and/or sorcery cards in your graveyard", "creature cards in all graveyards", "cards named {this} in all graveyards", "cards in all graveyards with the same name as that spell".</summary>
/// <exampledoc>Black Vise</exampledoc>
/// <examplecapture>cards in their hand</examplecapture>
[Dependent]
public class CardsInZone : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural("card"), Prop(Named), "in", Prop(Zone), Prop(Narrowed)];

    [Optional]
    public OneOf<ManyOf<TargetQualifier>, QualifierRun, AndOrQualifiers> Qualifiers { get; set; }
    [Optional]
    public NamedThis Named { get; set; }
    public PlayersZone Zone { get; set; }
    [Optional]
    public WithQualifier Narrowed { get; set; }
}
