namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of League Guildmage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>League Guildmage</exampledoc>
[Dependent]
public class LeagueGuildmageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy target instant or sorcery spell you control with mana value x"];
}
