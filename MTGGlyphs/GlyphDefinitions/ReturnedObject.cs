namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a return effect moves: a targeted card in a zone, a card named by where it is ("this card from your graveyard"), a particular permanent, a spell or a spell or permanent ("target spell or creature"), any one of a kind, any one card from a zone ("a creature card from your graveyard"), several targets ("up to two target creatures"), a whole group ("all attacking creatures", "each creature"), a number of permanents of a kind ("three creatures you control", "two islands you control"), or every card of a kind in a zone ("all instant and sorcery cards from your graveyard").</summary>
/// <exampledoc>Raise Dead</exampledoc>
/// <examplecapture>target creature card from your graveyard</examplecapture>
[Dependent]
public class ReturnedObject : GlyphOneOf
{
    public TargetCardInZone TargetCardInZone { get; set; }
    public PermanentFromZone PermanentFromZone { get; set; }
    public AllCardsFromZone AllCardsFromZone { get; set; }
    public TargetSpellOrPermanent TargetSpellOrPermanent { get; set; }
    public TargetSpell TargetSpell { get; set; }
    public PermanentPhrase PermanentPhrase { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
    public IndefiniteCardInZone IndefiniteCardInZone { get; set; }
    public CountedTargets CountedTargets { get; set; }
    public AllPermanents AllPermanents { get; set; }
    public EachPermanents EachPermanents { get; set; }
    public CountedPermanents CountedPermanents { get; set; }
}
