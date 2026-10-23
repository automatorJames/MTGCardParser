namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A hand size said of players picked out by "who": "has one or fewer cards in hand", "has more cards in hand than you".</summary>
/// <exampledoc>Bandit's Talent</exampledoc>
/// <examplecapture>has one or fewer cards in hand</examplecapture>
[Dependent]
public class HoldsHandSize : Glyph
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Size)];

    public HaveVerb Verb { get; set; }
    public OneOf<CardsInHandCount, ComparedHandSize> Size { get; set; }
}
