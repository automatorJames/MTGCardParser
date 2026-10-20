namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Who or what does something: a target or several ("x target creatures"), the enchanted permanent, a group of permanents ("white creatures", "all creatures", "each player", "each creature without flying", "each other attacking creature", "untapped creatures you control", "other goblins you control"), a player ("you", "each opponent", "that creature's controller", "a player with ten or more poison counters"), the card itself, "this creature" in reminder text, something named earlier ("it", "they", "that creature"), a permanent's activated abilities ("its activated abilities"), or any one permanent of a kind ("a creature with menace"). The first alternative that matches wins, so longer ones go first.</summary>
/// <exampledoc>Ancestral Recall</exampledoc>
/// <examplecapture>target player</examplecapture>
[Dependent]
public class Subject : GlyphOneOf
{
    public ActivatedAbilitiesOf ActivatedAbilities { get; set; }
    public SpecificTarget SpecificTarget { get; set; }
    public CountedTargets CountedTargets { get; set; }
    public EnchantedPermanent EnchantedPermanent { get; set; }
    public PlayerWithCounters PlayerWithCounters { get; set; }
    public WhichPlayer? Player { get; set; }
    public This This { get; set; }
    public PlayerReference PlayerReference { get; set; }
    public ThisCreature ThisCreature { get; set; }
    public AllPermanents AllPermanents { get; set; }
    public EachObject EachObject { get; set; }
    public EachPermanents EachPermanents { get; set; }
    public OtherPermanents OtherPermanents { get; set; }
    public Permanents Permanents { get; set; }
    public ThatCard ThatCard { get; set; }
    public It It { get; set; }
    public They They { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
}
