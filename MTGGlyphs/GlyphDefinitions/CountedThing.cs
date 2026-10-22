namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a "for each" counts: creatures in a party, counters (on an object, or removed this way), blockers or targets beyond the first, attachments ("aura attached to it"), colors of mana spent, objects acted on this way, basic land types, deaths, kicker payments, cards revealed from hand (amplify), permanents devoured, objects named earlier ("of those creatures"), players ("for each opponent"), distinct kinds among a group ("color among permanents you control"), or single points of damage or life ("1 damage prevented this way").</summary>
/// <exampledoc>Mephitic Ooze</exampledoc>
/// <examplecapture>artifact you control</examplecapture>
[Dependent]
public class CountedThing : GlyphOneOf
{
    public PartyCreatures PartyCreatures { get; set; }
    public CountersRemovedThisWay CountersRemovedThisWay { get; set; }
    public CountersOn CountersOn { get; set; }
    public BlockerBeyondFirst BlockerBeyondFirst { get; set; }
    public AttachedPermanents AttachedPermanents { get; set; }
    public CountedObjects CountedObjects { get; set; }
    public ColorOfManaSpent ColorOfManaSpent { get; set; }
    public AffectedThisWay AffectedThisWay { get; set; }
    public BasicLandTypesAmong BasicLandTypesAmong { get; set; }
    public TargetBeyondFirst TargetBeyondFirst { get; set; }
    public CreaturesThatDied CreaturesThatDied { get; set; }
    public TimesKicked TimesKicked { get; set; }
    public CardsRevealedFromHand CardsRevealedFromHand { get; set; }
    public PermanentsDevoured PermanentsDevoured { get; set; }
    public OfThoseObjects OfThoseObjects { get; set; }
    public CountedPlayers CountedPlayers { get; set; }
    public DistinctKinds DistinctKinds { get; set; }
    public PointOfDamageOrLife PointOfDamageOrLife { get; set; }
}
