namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Power Surge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Power Surge</exampledoc>
/// <examplecapture>this enchantment deals x damage to that player, where x is the number of untapped lands they controlled at the beginning of this turn</examplecapture>
[Dependent]
public class PowerSurgeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals x damage to that player, where x is the number of untapped lands they controlled at the beginning of this turn"];
}
