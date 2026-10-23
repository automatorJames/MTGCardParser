namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What damage is dealt to: a target and every permanent of a kind ("target player or planeswalker and each creature that player controls"), a targetable player or card, a player ("you"), a player or a permanent ("you or a creature you control", "a permanent or player", "that permanent or player"), every object of a kind ("each creature and each player", "each opponent and each creature they control", "each creature your opponents control", "each other creature"), every one dealt damage already ("each creature dealt damage this turn"), each of several targets ("each of two target creatures") or of a group named earlier ("each of them", "each of those creatures"), one named earlier - "that player", "that land's controller", "them" - whatever an attacker is attacking ("the player or planeswalker it's attacking"), the source itself ("deals damage to itself"), an indefinite permanent ("a planeswalker you control") or one particular permanent ("this creature", "it", "{this}", "enchanted creature").</summary>
/// <exampledoc>Copper Tablet</exampledoc>
/// <examplecapture>that player</examplecapture>
[Dependent]
public class Recipient : GlyphOneOf
{
    public TargetAndEach TargetAndEach { get; set; }
    public PlayerOrPermanent PlayerOrPermanent { get; set; }
    public PermanentOrPlayer PermanentOrPlayer { get; set; }
    public ThatPermanentOrPlayer ThatPermanentOrPlayer { get; set; }
    public Target Target { get; set; }
    public PlayerReference PlayerReference { get; set; }
    public ThatCard ThatCard { get; set; }
    public WhichPlayer? Player { get; set; }
    public ThemPlayer ThemPlayer { get; set; }
    public AttackedByIt AttackedByIt { get; set; }
    public EachDamagedObject EachDamaged { get; set; }
    public ManyOf<OneOf<EachObject, EachPermanents>> EachObjects { get; set; }
    public EachObject EachObject { get; set; }
    public EachPermanents EachPermanents { get; set; }
    public EachOfTargets EachOfTargets { get; set; }
    public EachOfThem EachOfThem { get; set; }
    public Itself Itself { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
    public PermanentPhrase PermanentPhrase { get; set; }
}
