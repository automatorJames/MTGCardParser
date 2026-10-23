namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Oblivious Bookworm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oblivious Bookworm</exampledoc>
/// <examplecapture>if you do, discard a card unless a permanent entered the battlefield face down under your control this turn or you turned a permanent face up this turn</examplecapture>
[TokenizationOrder(-1)]
public class ObliviousBookwormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, discard a card unless a permanent entered the battlefield face down under your control this turn or you turned a permanent face up this turn"];
}
