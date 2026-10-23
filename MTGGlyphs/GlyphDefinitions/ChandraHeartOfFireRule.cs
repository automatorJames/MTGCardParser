namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chandra, Heart of Fire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra, Heart of Fire</exampledoc>
[TokenizationOrder(-1)]
public class ChandraHeartOfFireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add six {r}"];
}
