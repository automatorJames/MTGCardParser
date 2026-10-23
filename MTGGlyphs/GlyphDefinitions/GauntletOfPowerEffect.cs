namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gauntlet of Power, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gauntlet of Power</exampledoc>
[Dependent]
public class GauntletOfPowerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its controller adds an additional one mana of that color"];
}
