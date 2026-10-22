namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Who or what does something: a target's controller or owner ("target creature's owner"), a target or several ("x target creatures"), a target with others related to it ("target creature and all other creatures with the same name as that creature"), a targeted card outside the battlefield ("target instant or sorcery card in your graveyard"), a target that may be a spell or a permanent ("target spell or permanent"), the enchanted permanent, a group of permanents ("white creatures", "all creatures", "each player", "each creature without flying", "each other attacking creature", "each creature you control with a +1/+1 counter on it", "untapped creatures you control", "other goblins you control"), a player ("you", "each opponent", "that creature's controller", "a player with ten or more poison counters"), the card itself, "this creature" in reminder text, something named earlier ("it", "they", "he", "she", "that creature", "that token"), a spell picked by order ("the next spell you cast this turn"), a soulbond pair ("both creatures"), a permanent's activated abilities ("its activated abilities"), any one permanent of a kind ("a creature with menace"), or, last of all, a legend's short name before its verb ("akroma gets +1/+0"). The first alternative that matches wins, so longer ones go first.</summary>
/// <exampledoc>Ancestral Recall</exampledoc>
/// <examplecapture>target player</examplecapture>
[Dependent]
public class Subject : GlyphOneOf
{
    public ActivatedAbilitiesOf ActivatedAbilities { get; set; }
    public TargetsController TargetsController { get; set; }
    public TargetSpellOrPermanent TargetSpellOrPermanent { get; set; }
    public TargetCardInZone TargetCardInZone { get; set; }
    public TargetAndOthers TargetAndOthers { get; set; }
    public SpecificTarget SpecificTarget { get; set; }
    public CountedTargets CountedTargets { get; set; }
    public OrdinalSpellCast OrdinalSpellCast { get; set; }
    public EnchantedPermanent EnchantedPermanent { get; set; }
    public PlayerWithCounters PlayerWithCounters { get; set; }
    public WhichPlayer? Player { get; set; }
    public This This { get; set; }
    public PlayerReference PlayerReference { get; set; }
    public ThisCreature ThisCreature { get; set; }
    public AllPermanents AllPermanents { get; set; }
    public EachControlledPermanent EachControlled { get; set; }
    public PairedCreatures PairedCreatures { get; set; }
    public EachObject EachObject { get; set; }
    public EachPermanents EachPermanents { get; set; }
    public OtherPermanents OtherPermanents { get; set; }
    public Permanents Permanents { get; set; }
    public ThatCard ThatCard { get; set; }
    public ThatToken ThatToken { get; set; }
    public It It { get; set; }
    public They They { get; set; }
    public PersonalPronoun PersonalPronoun { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
    public ShortNameSubject ShortName { get; set; }
}
