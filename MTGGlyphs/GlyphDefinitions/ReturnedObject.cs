namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a return effect moves: a targeted card in a zone, several targeted cards of different kinds from one zone ("up to one target instant card and up to one target sorcery card from your graveyard"), a card named by where it is ("this card from your graveyard"), a particular permanent, a spell or a spell or permanent ("target spell or creature"), any one of a kind, any one card from a zone ("a creature card from your graveyard"), a number of cards from a zone ("two cards at random from your graveyard"), several targets ("up to two target creatures"), two targets together ("target creature and target land"), a target and what goes with it ("target nonland permanent and all other permanents with the same name as that permanent"), a whole group ("all attacking creatures", "each creature"), a number of permanents of a kind ("three creatures you control", "two islands you control"), every card of a kind in a zone ("all instant and sorcery cards from your graveyard"), or, last, a legend's short name for itself ("return arcanis to its owner's hand").</summary>
/// <exampledoc>Raise Dead</exampledoc>
/// <examplecapture>target creature card from your graveyard</examplecapture>
[Dependent]
public class ReturnedObject : GlyphOneOf
{
    public TargetCardInZone TargetCardInZone { get; set; }
    public TargetCardsOfKindsInZone TargetCardsOfKinds { get; set; }
    public PermanentFromZone PermanentFromZone { get; set; }
    public AllCardsFromZone AllCardsFromZone { get; set; }
    public TargetSpellOrPermanent TargetSpellOrPermanent { get; set; }
    public TargetSpell TargetSpell { get; set; }
    public OneOf<PermanentPair, TargetAndOthers> TwoOrMore { get; set; }
    public PermanentPhrase PermanentPhrase { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
    public IndefiniteCardInZone IndefiniteCardInZone { get; set; }
    public CountedCardsInZone CountedCardsInZone { get; set; }
    public CountedTargets CountedTargets { get; set; }
    public AllPermanents AllPermanents { get; set; }
    public EachPermanents EachPermanents { get; set; }
    public CountedPermanents CountedPermanents { get; set; }
    public ShortName ShortName { get; set; }
}
