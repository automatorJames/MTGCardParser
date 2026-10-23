namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spawnsire of Ulamog, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spawnsire of Ulamog</exampledoc>
[Dependent]
public class SpawnsireOfUlamogEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cast any number of eldrazi spells from among cards you own outside the game without paying their mana costs"];
}
