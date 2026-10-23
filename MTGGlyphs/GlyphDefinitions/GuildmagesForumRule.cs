namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Guildmages' Forum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guildmages' Forum</exampledoc>
[TokenizationOrder(-1)]
public class GuildmagesForumRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that mana is spent on a multicolored creature spell, that creature enters with an additional +1/+1 counter on it"];
}
