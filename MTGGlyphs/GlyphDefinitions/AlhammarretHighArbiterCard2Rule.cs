namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Alhammarret, High Arbiter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alhammarret, High Arbiter</exampledoc>
[TokenizationOrder(-1)]
public class AlhammarretHighArbiterCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you choose the name of a nonland card revealed this way"];
}
