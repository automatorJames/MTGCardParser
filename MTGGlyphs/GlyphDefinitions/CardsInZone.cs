namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards in a player's zone, to count, optionally of a type: "cards in their hand", "creature cards in your graveyard", "(each) card in your hand".</summary>
/// <exampledoc>Black Vise</exampledoc>
/// <examplecapture>cards in their hand</examplecapture>
[Dependent]
public class CardsInZone : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural("card"), "in", Prop(Zone)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    public PlayersZone Zone { get; set; }
}
