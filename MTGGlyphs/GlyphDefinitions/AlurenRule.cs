namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aluren, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aluren</exampledoc>
[TokenizationOrder(-1)]
public class AlurenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any player may cast creature spells with mana value 3 or less without paying their mana costs and as though they had flash"];
}
