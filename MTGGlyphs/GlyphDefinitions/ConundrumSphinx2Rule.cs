namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Conundrum Sphinx, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Conundrum Sphinx</exampledoc>
[TokenizationOrder(-1)]
public class ConundrumSphinx2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it doesn't, that player puts it on the bottom of their library"];
}
