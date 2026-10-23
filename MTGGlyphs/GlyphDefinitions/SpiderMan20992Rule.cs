namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spider-Man 2099, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Man 2099</exampledoc>
[TokenizationOrder(-1)]
public class SpiderMan20992Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["from the future — you can't cast", Nib.This, "during your first, second, or third turns of the game"];
}
