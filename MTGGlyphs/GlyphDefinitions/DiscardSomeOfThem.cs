namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Discarding (or otherwise handling by a card verb) some of the cards just drawn or named: "discard one of them", "discard two of those cards".</summary>
/// <exampledoc>Casting of Bones</exampledoc>
/// <examplecapture>discard one of them</examplecapture>
[Dependent]
public class DiscardSomeOfThem : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(CardVerb), Prop(Cards)];

    public CardVerb CardVerb { get; set; }
    public CountOfThem Cards { get; set; }
}
