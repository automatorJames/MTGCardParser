namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kraken of the Straits, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kraken of the Straits</exampledoc>
[TokenizationOrder(-1)]
public class KrakenOfTheStraitsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures with power less than the number of islands you control can't block this creature"];
}
