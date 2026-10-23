namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aether Gust, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aether Gust</exampledoc>
[TokenizationOrder(-1)]
public class AetherGustRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target spell or permanent that's red or green"];
}
