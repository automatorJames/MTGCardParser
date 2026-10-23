namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Garruk's Horde, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Garruk's Horde</exampledoc>
[TokenizationOrder(-1)]
public class GarruksHorde2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you still pay the spell's costs"];
}
