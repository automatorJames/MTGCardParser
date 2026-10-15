namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Drawing or discarding a number of cards: "draw two cards", "discard three cards", "discard x cards at random", "discard a black card". Discarding is also a cost: "{t}, discard a card: …".</summary>
/// <exampledoc>Bazaar of Baghdad</exampledoc>
/// <examplecapture>draw two cards</examplecapture>
[Dependent]
public class DrawOrDiscardCards : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(CardVerb), Prop(Quantity), Prop(Qualifier), Plural("card"), Prop(AtRandom)];

    public CardVerb CardVerb { get; set; }
    public Quantity Quantity { get; set; }
    [Optional]
    public TargetQualifier Qualifier { get; set; }
    [RegexPattern("at random")]
    public bool AtRandom { get; set; }
}
