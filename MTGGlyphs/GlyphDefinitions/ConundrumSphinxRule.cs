namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Conundrum Sphinx, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Conundrum Sphinx</exampledoc>
[TokenizationOrder(-1)]
public class ConundrumSphinxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the card a player revealed has the name they chose, that player puts it into their hand"];
}
