namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hall of the Bandit Lord, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hall of the Bandit Lord</exampledoc>
[TokenizationOrder(-1)]
public class HallOfTheBanditLordRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that mana is spent on a creature spell, it gains haste"];
}
