namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spelltwine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spelltwine</exampledoc>
[TokenizationOrder(-1)]
public class SpelltwineRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cast the copies if able without paying their mana costs"];
}
