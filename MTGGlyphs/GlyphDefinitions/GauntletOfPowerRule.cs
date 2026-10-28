namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gauntlet of Power, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gauntlet of Power</exampledoc>
[TokenizationOrder(-1)]
public class GauntletOfPowerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a basic land is tapped for mana of the chosen color, its controller adds an additional one mana of that color"];
}
