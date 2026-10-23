namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pledge of Loyalty, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pledge of Loyalty</exampledoc>
[TokenizationOrder(-1)]
public class PledgeOfLoyaltyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature has protection from each color among permanents you control"];
}
