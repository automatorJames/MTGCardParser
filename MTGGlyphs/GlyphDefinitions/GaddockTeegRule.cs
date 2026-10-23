namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gaddock Teeg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gaddock Teeg</exampledoc>
[TokenizationOrder(-1)]
public class GaddockTeegRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["noncreature spells with {x} in their mana costs can't be cast"];
}
