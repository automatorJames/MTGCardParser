namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fugitive Codebreaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fugitive Codebreaker</exampledoc>
[TokenizationOrder(-1)]
public class FugitiveCodebreakerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this cost is reduced by {1} for each instant and sorcery card in your graveyard"];
}
