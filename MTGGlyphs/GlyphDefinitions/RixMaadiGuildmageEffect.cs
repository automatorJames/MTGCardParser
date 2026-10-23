namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rix Maadi Guildmage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rix Maadi Guildmage</exampledoc>
[Dependent]
public class RixMaadiGuildmageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player who lost life this turn loses 1 life"];
}
