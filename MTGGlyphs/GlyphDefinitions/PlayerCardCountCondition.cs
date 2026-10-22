namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"an opponent has eight or more cards in their graveyard", "you have seven or more cards in your graveyard": a player's count of cards in a zone reaching a threshold.</summary>
/// <exampledoc>Tenured Oilcaster</exampledoc>
/// <examplecapture>an opponent has eight or more cards in their graveyard</examplecapture>
[Dependent]
public class PlayerCardCountCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Pattern("(has|have)"), Prop(Count), Prop(Cards)];

    public WhichPlayer Player { get; set; }
    public AtLeastCount Count { get; set; }
    public CardsInZone Cards { get; set; }
}
