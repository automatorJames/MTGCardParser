namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nightmare Incursion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nightmare Incursion</exampledoc>
[TokenizationOrder(-1)]
public class NightmareIncursionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search target player's library for up to x cards, where x is the number of swamps you control, and exile them"];
}
