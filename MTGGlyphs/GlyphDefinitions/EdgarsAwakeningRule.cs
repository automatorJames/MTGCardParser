namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Edgar's Awakening, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Edgar's Awakening</exampledoc>
[TokenizationOrder(-1)]
public class EdgarsAwakeningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you discard this card, you may pay {b}"];
}
