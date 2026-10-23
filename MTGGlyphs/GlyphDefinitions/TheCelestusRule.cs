namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Celestus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Celestus</exampledoc>
[TokenizationOrder(-1)]
public class TheCelestusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, it becomes night"];
}
