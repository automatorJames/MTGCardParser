namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Izzet Guildmage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Izzet Guildmage</exampledoc>
[Dependent]
public class IzzetGuildmageEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy target instant spell you control with mana value 2 or less"];
}
