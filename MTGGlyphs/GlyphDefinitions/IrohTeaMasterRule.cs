namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Iroh, Tea Master, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Iroh, Tea Master</exampledoc>
[TokenizationOrder(-1)]
public class IrohTeaMasterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on that token for each permanent you own that your opponents control"];
}
