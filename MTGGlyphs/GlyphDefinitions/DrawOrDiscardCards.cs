namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Drawing or discarding a number of cards: "draw two cards", "discard three cards", "discard x cards at random", "discard a black card", "discard a red or green card", "discards a noncreature, nonland card", "draws two additional cards", "draws up to seven cards", or cards picked by an earlier choice ("discards all cards of that color", "discards all cards with that name"). Discarding is also a cost: "{t}, discard a card: …". It also stands alone as a spell's instruction: "draw a card", and as the alternative in "sacrifice a permanent or discard a card".</summary>
/// <exampledoc>Bazaar of Baghdad</exampledoc>
/// <examplecapture>draw two cards</examplecapture>
public class DrawOrDiscardCards : Glyph, IAlternativePredicate, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(CardVerb), Prop(UpTo), Prop(Quantity), Prop(Additional), Prop(Qualifier), Plural("card"), Prop(Chosen), Prop(AtRandom)];

    public CardVerb CardVerb { get; set; }
    [Optional]
    [RegexPattern("up to")]
    public bool? UpTo { get; set; }
    public Quantity Quantity { get; set; }
    [Optional]
    [RegexPattern("additional")]
    public bool? Additional { get; set; }
    [Optional]
    public OneOf<TargetQualifier, CompoundOf<QualifierWords>> Qualifier { get; set; }
    [Optional]
    public OneOf<WithName, ChosenAspect?> Chosen { get; set; }
    [RegexPattern("at random")]
    public bool AtRandom { get; set; }
}
