namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the cards in their hand", "the non-demon permanents they control of their choice": what a halving action takes half of, when it's a set of objects.</summary>
/// <exampledoc>Curse of the Cabal</exampledoc>
/// <examplecapture>the permanents they control of their choice</examplecapture>
[Dependent]
public class HalvedObjects : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Objects), Prop(OfTheirChoice)];

    public OneOf<CardsInZone, Permanents> Objects { get; set; }
    [RegexPattern("of their choice")]
    public bool OfTheirChoice { get; set; }
}
