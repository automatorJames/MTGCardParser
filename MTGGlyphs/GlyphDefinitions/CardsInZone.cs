namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards in a player's zone, to count, optionally of a type or a choice of types: "cards in their hand", "creature cards in your graveyard", "(each) card in your hand", "instant and sorcery cards in your graveyard", "creature cards in all graveyards".</summary>
/// <exampledoc>Black Vise</exampledoc>
/// <examplecapture>cards in their hand</examplecapture>
[Dependent]
public class CardsInZone : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural("card"), "in", Prop(Zone)];

    [Optional]
    public OneOf<ManyOf<TargetQualifier>, QualifierRun> Qualifiers { get; set; }
    public PlayersZone Zone { get; set; }
}
