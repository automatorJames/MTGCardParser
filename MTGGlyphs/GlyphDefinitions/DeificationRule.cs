namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Deification, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Deification</exampledoc>
[TokenizationOrder(-1)]
public class DeificationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as this enchantment enters, choose a planeswalker type"];
}
