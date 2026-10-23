namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mystic Reflection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mystic Reflection</exampledoc>
[TokenizationOrder(-1)]
public class MysticReflectionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time one or more creatures or planeswalkers enter this turn, they enter as copies of the chosen creature"];
}
