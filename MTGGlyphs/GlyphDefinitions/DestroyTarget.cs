namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Destroying or exiling one permanent or several, a card in another zone, or a spell: "destroy target land", "exile two target artifacts", "exile all creatures", "destroy target creature and target land", "destroy up to one target artifact, up to one target creature, and up to one target enchantment", "destroy target artifact and all other artifacts with the same name as that artifact", "exile target card from an opponent's graveyard", "exile a creature card from your graveyard", "exile a merfolk you control or a merfolk card from your hand", "exile target creature an opponent controls until this creature leaves the battlefield", "destroy that creature at end of combat", "exile target spell with mana value 4 or less", "exile it with three time counters on it" (suspend), "exile a card from your hand face down", "exile this card from your hand face down" (foretell), "destroy one of them at random", and as a cost "exile {this}", "exile this card from your graveyard". Also the second of two actions: "… and exile that card". The verb is a <see cref="RemovalVerb"/>.</summary>
/// <exampledoc>Ice Storm</exampledoc>
/// <examplecapture>destroy target land</examplecapture>
public class DestroyTarget : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Destroyed), Prop(WithCounters), Prop(Until), Prop(AtEndOfCombat), Prop(Manner)];

    public RemovalVerb Verb { get; set; }
    public OneOf<TargetCardInZone, OneOf<PermanentFromZone, TargetSpell>, OneOf<PermanentPhrase, RemovedObjects, OneOf<OneOf<PermanentPair, TargetList>, TargetAndOthers, PermanentOrCard>>> Destroyed { get; set; }
    [Optional]
    public EntersWithCounters WithCounters { get; set; }
    [Optional]
    public UntilThisLeaves Until { get; set; }
    [RegexPattern("at end of combat")]
    public bool AtEndOfCombat { get; set; }
    [Optional]
    public RemovalManner? Manner { get; set; }
}
