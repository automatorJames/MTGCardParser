namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Destroying or exiling one permanent or several, or a card in another zone: "destroy target land", "exile two target artifacts", "exile all creatures", "destroy target creature and target land", "destroy target artifact and all other artifacts with the same name as that artifact", "exile target card from an opponent's graveyard", "exile a creature card from your graveyard", "exile target creature an opponent controls until this creature leaves the battlefield", "destroy that creature at end of combat", "exile it with three time counters on it" (suspend), and as a cost "exile {this}", "exile this card from your graveyard". Also the second of two actions: "… and exile that card". The verb is a <see cref="RemovalVerb"/>.</summary>
/// <exampledoc>Ice Storm</exampledoc>
/// <examplecapture>destroy target land</examplecapture>
public class DestroyTarget : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Destroyed), Prop(WithCounters), Prop(Until), Prop(AtEndOfCombat)];

    public RemovalVerb Verb { get; set; }
    public OneOf<TargetCardInZone, PermanentFromZone, OneOf<PermanentPhrase, RemovedObjects, OneOf<PermanentPair, TargetAndOthers>>> Destroyed { get; set; }
    [Optional]
    public EntersWithCounters WithCounters { get; set; }
    [Optional]
    public UntilThisLeaves Until { get; set; }
    [RegexPattern("at end of combat")]
    public bool AtEndOfCombat { get; set; }
}
