namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Charging War Boar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Charging War Boar</exampledoc>
[Dependent]
public class ChargingWarBoarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it can deal excess damage to the player or planeswalker it's attacking"];
}
