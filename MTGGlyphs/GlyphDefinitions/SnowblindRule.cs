namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Snowblind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Snowblind</exampledoc>
[TokenizationOrder(-1)]
public class SnowblindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["y is equal to x or to enchanted creature's toughness minus 1, whichever is smaller"];
}
