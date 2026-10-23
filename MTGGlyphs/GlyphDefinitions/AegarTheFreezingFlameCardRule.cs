namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aegar, the Freezing Flame, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aegar, the Freezing Flame</exampledoc>
[TokenizationOrder(-1)]
public class AegarTheFreezingFlameCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature or planeswalker an opponent controls is dealt excess damage, if a giant, wizard, or spell you controlled dealt damage to it this turn, draw a card"];
}
