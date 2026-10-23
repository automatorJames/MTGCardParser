namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Preferred Selection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Preferred Selection</exampledoc>
[TokenizationOrder(-1)]
public class PreferredSelectionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may sacrifice this enchantment and pay {2}{g}{g}"];
}
