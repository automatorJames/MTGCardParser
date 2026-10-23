namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Serra Bestiary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Serra Bestiary</exampledoc>
[TokenizationOrder(-1)]
public class SerraBestiaryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature can't attack or block, and its activated abilities with {t} in their costs can't be activated"];
}
