namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mila, Crafty Companion // Lukka, Wayward Bonder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mila, Crafty Companion // Lukka, Wayward Bonder</exampledoc>
[TokenizationOrder(-1)]
public class MilaCraftyCompanionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an opponent attacks one or more planeswalkers you control, put a loyalty counter on each planeswalker you control"];
}
