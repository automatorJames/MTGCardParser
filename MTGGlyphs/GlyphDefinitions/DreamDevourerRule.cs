namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dream Devourer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dream Devourer</exampledoc>
[TokenizationOrder(-1)]
public class DreamDevourerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its foretell cost is equal to its mana cost reduced by {2}"];
}
