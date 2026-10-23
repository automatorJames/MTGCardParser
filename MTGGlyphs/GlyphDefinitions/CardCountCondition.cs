namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"there are seven or more cards in your graveyard", "there are fewer than six creature cards in your graveyard", "there are no cards in your library": a count of cards in a zone reaching a threshold (threshold), staying under one, or there being none.</summary>
/// <exampledoc>Anurid Barkripper</exampledoc>
/// <examplecapture>there are seven or more cards in your graveyard</examplecapture>
[Dependent]
public class CardCountCondition : Glyph
{
    public override Nib[] Nibs => ["there are", Prop(Count), Prop(Cards)];

    public OneOf<AtLeastCount, FewerThanCount, NoneCount> Count { get; set; }
    public CardsInZone Cards { get; set; }
}
