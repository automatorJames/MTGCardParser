namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of United Battlefront, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>United Battlefront</exampledoc>
[TokenizationOrder(-1)]
public class UnitedBattlefrontRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put up to two noncreature, nonland permanent cards with mana value 3 or less from among them onto the battlefield"];
}
