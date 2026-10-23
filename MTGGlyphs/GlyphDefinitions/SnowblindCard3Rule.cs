namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Snowblind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Snowblind</exampledoc>
[TokenizationOrder(-1)]
public class SnowblindCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature gets -x/-y"];
}
