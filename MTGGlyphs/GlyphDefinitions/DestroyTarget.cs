namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Destroying or exiling one permanent or several, or a card in another zone: "destroy target land", "exile two target artifacts", "exile all creatures", "exile target card from an opponent's graveyard", "exile a creature card from your graveyard", "exile target creature an opponent controls until this creature leaves the battlefield", "destroy that creature at end of combat", "exile it with three time counters on it" (suspend), and as a cost "exile {this}", "exile this card from your graveyard". Also the second of two actions: "… and exile that card". The verb is a <see cref="RemovalVerb"/>.</summary>
/// <exampledoc>Ice Storm</exampledoc>
/// <examplecapture>destroy target land</examplecapture>
public class DestroyTarget : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Destroyed), Prop(WithCounters), Prop(Until), Prop(AtEndOfCombat)];

    public RemovalVerb Verb { get; set; }
    public OneOf<TargetCardInZone, PermanentFromZone, OneOf<PermanentPhrase, RemovedObjects>> Destroyed { get; set; }
    [Optional]
    public EntersWithCounters WithCounters { get; set; }
    [Optional]
    public UntilThisLeaves Until { get; set; }
    [RegexPattern("at end of combat")]
    public bool AtEndOfCombat { get; set; }
}
