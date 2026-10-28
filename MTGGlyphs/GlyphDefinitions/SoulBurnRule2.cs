namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Soul Burn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul Burn</exampledoc>
/// <examplecapture>you gain life equal to the damage dealt, but not more than the amount of {b} spent on x, the player's life total before the damage was dealt, the planeswalker's loyalty before the damage was dealt, or the creature's toughness</examplecapture>
[TokenizationOrder(-1)]
public class SoulBurnRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain life equal to the damage dealt, but not more than the amount of {b} spent on x, the player's life total before the damage was dealt, the planeswalker's loyalty before the damage was dealt, or the creature's toughness"];
}
