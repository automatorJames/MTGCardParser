namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Guild Feud, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guild Feud</exampledoc>
[TokenizationOrder(-1)]
public class GuildFeudRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you do the same with the top three cards of your library"];
}
