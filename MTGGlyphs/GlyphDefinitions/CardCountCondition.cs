namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"there are seven or more cards in your graveyard", "there are fewer than six creature cards in your graveyard", "there are no cards in your library", "there are three or more cards exiled with this creature", "there are cards exiled with this enchantment": a count of cards in a zone (or exiled with this permanent) reaching a threshold (threshold), staying under one, there being none, or there being any.</summary>
/// <exampledoc>Anurid Barkripper</exampledoc>
/// <examplecapture>there are seven or more cards in your graveyard</examplecapture>
[Dependent]
public class CardCountCondition : Glyph
{
    public override Nib[] Nibs => ["there are", Prop(Count), Prop(Cards)];

    [Optional]
    public OneOf<AtLeastCount, FewerThanCount, NoneCount> Count { get; set; }
    public OneOf<CardsInZone, CardsExiledWithThis> Cards { get; set; }
}
