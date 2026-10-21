namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One particular permanent or card: {this}, "this spell" (the card on the stack), "this permanent" in reminder text, "it", a target, one named earlier ("that creature", "the exiled card", "that token"), the enchanted one, the cards left from a reveal ("all other cards revealed this way"), or a legend's short name at the end of a clause. "this spell" goes before ThisCreature, whose bare "this" would stop short of it.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>enchanted creature</examplecapture>
[Dependent]
public class PermanentPhrase : GlyphOneOf
{
    public This This { get; set; }
    public ThisSpell ThisSpell { get; set; }
    public ThisCreature ThisCreature { get; set; }
    public It It { get; set; }
    public SpecificTarget SpecificTarget { get; set; }
    public ThatCard ThatCard { get; set; }
    public EnchantedPermanent EnchantedPermanent { get; set; }
    public ThatCardPhrase ThatCardPhrase { get; set; }
    public ThatToken ThatToken { get; set; }
    public OtherCardsRevealedThisWay OtherCardsRevealedThisWay { get; set; }
    public ShortNameObject ShortName { get; set; }
}
