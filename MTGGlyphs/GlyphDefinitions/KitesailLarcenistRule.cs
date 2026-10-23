namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kitesail Larcenist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kitesail Larcenist</exampledoc>
/// <examplecapture>for as long as this creature remains on the battlefield, the chosen permanents become treasure artifacts with "{t}, sacrifice this artifact: add one mana of any color" and lose all other abilities</examplecapture>
[TokenizationOrder(-1)]
public class KitesailLarcenistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for as long as this creature remains on the battlefield, the chosen permanents become treasure artifacts with \"{t}, sacrifice this artifact: add one mana of any color\" and lose all other abilities"];
}
