namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The stop condition of a reveal-until loop, said of the card: "a creature card is revealed".</summary>
/// <exampledoc>Blessed Reincarnation</exampledoc>
/// <examplecapture>a creature card is revealed</examplecapture>
[Dependent]
public class CardIsFound : Glyph
{
    public override Nib[] Nibs => [Prop(Card), "is", Alt("revealed", "exiled")];

    public AnyCard Card { get; set; }
}
