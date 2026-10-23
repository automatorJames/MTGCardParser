namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Snowblind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Snowblind</exampledoc>
[TokenizationOrder(-1)]
public class SnowblindCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, x is the number of snow lands its controller controls"];
}
