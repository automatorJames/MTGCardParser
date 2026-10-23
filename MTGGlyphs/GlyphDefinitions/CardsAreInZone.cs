namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A count of cards in a zone reaching a threshold, stated with the cards as subject: "twenty or more creature cards are in your graveyard", "eight or more cards are in your graveyard". The "there are …" wording is <see cref="CardCountCondition"/>.</summary>
/// <exampledoc>Mortal Combat</exampledoc>
/// <examplecapture>twenty or more creature cards are in your graveyard</examplecapture>
[Dependent]
public class CardsAreInZone : Glyph
{
    public override Nib[] Nibs => [Prop(Count), Prop(Qualifiers), "cards are in", Prop(Zone)];

    public OneOf<AtLeastCount, FewerThanCount> Count { get; set; }
    [Optional]
    public CompoundOf<QualifierWords> Qualifiers { get; set; }
    public PlayersZone Zone { get; set; }
}
