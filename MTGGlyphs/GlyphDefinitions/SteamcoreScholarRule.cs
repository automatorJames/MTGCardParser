namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Steamcore Scholar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Steamcore Scholar</exampledoc>
[TokenizationOrder(-1)]
public class SteamcoreScholarRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then discard two cards unless you discard an instant or sorcery card or a creature card with flying"];
}
