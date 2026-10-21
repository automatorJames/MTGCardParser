namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a return effect moves: a targeted card in a zone, a card named by where it is ("this card from your graveyard"), a particular permanent, any one of a kind, any one card from a zone ("a creature card from your graveyard"), several targets ("up to two target creatures"), or a whole group ("all attacking creatures", "each creature").</summary>
/// <exampledoc>Raise Dead</exampledoc>
/// <examplecapture>target creature card from your graveyard</examplecapture>
[Dependent]
public class ReturnedObject : GlyphOneOf
{
    public TargetCardInZone TargetCardInZone { get; set; }
    public PermanentFromZone PermanentFromZone { get; set; }
    public PermanentPhrase PermanentPhrase { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
    public IndefiniteCardInZone IndefiniteCardInZone { get; set; }
    public CountedTargets CountedTargets { get; set; }
    public AllPermanents AllPermanents { get; set; }
    public EachPermanents EachPermanents { get; set; }
}
