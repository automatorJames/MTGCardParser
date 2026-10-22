namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What is played or cast: a card named earlier ("that card", "it", "them"), a copy, a target card in a zone ("target instant or sorcery card from an opponent's graveyard"), spells of a kind, or any card or land ("plays a land", "play a card"). Not "this spell": "you cast this spell" is a trigger condition (CastingThisSpell).</summary>
/// <exampledoc>Isochron Scepter</exampledoc>
/// <examplecapture>the copy</examplecapture>
[Dependent]
public class CastObject : GlyphOneOf
{
    public TargetCardInZone TargetCardInZone { get; set; }
    public ThatCardPhrase ThatCardPhrase { get; set; }
    public It It { get; set; }
    public Them Them { get; set; }
    public TheCopy TheCopy { get; set; }
    public CastableSpells CastableSpells { get; set; }
    public SpellsOfKind SpellsOfKind { get; set; }
    public AnyCard AnyCard { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
}
