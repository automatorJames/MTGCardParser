namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"there are seven or more cards in your graveyard": a count of cards in a zone reaching a threshold (threshold).</summary>
/// <exampledoc>Anurid Barkripper</exampledoc>
/// <examplecapture>there are seven or more cards in your graveyard</examplecapture>
[Dependent]
public class CardCountCondition : Glyph
{
    public override Nib[] Nibs => ["there are", Prop(Count), Prop(Cards)];

    public AtLeastCount Count { get; set; }
    public CardsInZone Cards { get; set; }
}
