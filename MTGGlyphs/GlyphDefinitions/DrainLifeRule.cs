namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Drain Life, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Drain Life</exampledoc>
/// <examplecapture>you gain life equal to the damage dealt, but not more life than the player's life total before the damage was dealt, the planeswalker's loyalty before the damage was dealt, or the creature's toughness</examplecapture>
[TokenizationOrder(-1)]
public class DrainLifeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain life equal to the damage dealt, but not more life than the player's life total before the damage was dealt, the planeswalker's loyalty before the damage was dealt, or the creature's toughness"];
}
