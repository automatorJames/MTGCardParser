namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a "for each" counts: counters, objects, attachments ("aura attached to it"), blockers or targets beyond the first, colors of mana spent, objects acted on this way, basic land types, deaths, kicker payments, cards revealed from hand (amplify), or objects named earlier ("of those creatures").</summary>
/// <exampledoc>Mephitic Ooze</exampledoc>
/// <examplecapture>artifact you control</examplecapture>
[Dependent]
public class CountedThing : GlyphOneOf
{
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
    public OfThoseObjects OfThoseObjects { get; set; }
}
