namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mila, Crafty Companion // Lukka, Wayward Bonder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mila, Crafty Companion // Lukka, Wayward Bonder</exampledoc>
[Dependent]
public class MilaCraftyCompanionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a loyalty counter on each planeswalker you control"];
}
