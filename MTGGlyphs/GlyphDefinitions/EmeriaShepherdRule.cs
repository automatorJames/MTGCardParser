namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Emeria Shepherd, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Emeria Shepherd</exampledoc>
[TokenizationOrder(-1)]
public class EmeriaShepherdRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that land is a plains, you may return that nonland permanent card to the battlefield instead"];
}
