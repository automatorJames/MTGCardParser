namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Guild Feud, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guild Feud</exampledoc>
[TokenizationOrder(-1)]
public class GuildFeudRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two creatures are put onto the battlefield this way, those creatures fight each other"];
}
