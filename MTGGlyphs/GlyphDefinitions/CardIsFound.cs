namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The stop condition of a reveal-until loop, said of the card: "a creature card is revealed", "two land cards are revealed".</summary>
/// <exampledoc>Blessed Reincarnation</exampledoc>
/// <examplecapture>a creature card is revealed</examplecapture>
[Dependent]
public class CardIsFound : Glyph
{
    public override Nib[] Nibs => [Prop(Card), Alt("is", "are"), Alt("revealed", "exiled")];

    public OneOf<AnyCard, CountedCards> Card { get; set; }
}
