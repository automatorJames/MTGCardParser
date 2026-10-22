namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the sacrificed creature", "the sacrificed creatures", "the exiled creature", "the revealed card", "the revealed cards", "the discarded card": the object a cost or an earlier effect handled, named again. (Plain "the exiled card" is <see cref="ThatCardPhrase"/>.)</summary>
/// <exampledoc>Diamond Valley</exampledoc>
/// <examplecapture>the sacrificed creature</examplecapture>
[Dependent]
public class SacrificedPermanent : Glyph
{
    public override Nib[] Nibs => ["the", Prop(How), Plural(Prop(CardType)), Prop(Card)];

    public HandledHow How { get; set; }
    [Optional]
    public CardType? CardType { get; set; }
    [Optional]
    [RegexPattern("cards?")]
    public bool Card { get; set; }
}
