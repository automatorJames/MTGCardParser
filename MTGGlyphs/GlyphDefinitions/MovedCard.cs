namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a "put" effect moves: a particular permanent, a card named earlier, a targeted card in a zone ("target creature card from a graveyard"), any card of a kind in a zone ("a land card from your hand"), or cards picked out of a group ("a creature card from among them").</summary>
/// <exampledoc>Endless Obedience</exampledoc>
/// <examplecapture>target creature card from a graveyard</examplecapture>
[Dependent]
public class MovedCard : GlyphOneOf
{
    public TargetCardInZone TargetCardInZone { get; set; }
    public IndefiniteCardInZone IndefiniteCardInZone { get; set; }
    public CardsFromAmong CardsFromAmong { get; set; }
    public PermanentPhrase PermanentPhrase { get; set; }
    public ThatCard ThatCard { get; set; }
}
