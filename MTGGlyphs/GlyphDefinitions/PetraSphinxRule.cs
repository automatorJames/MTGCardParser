namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Petra Sphinx, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Petra Sphinx</exampledoc>
[TokenizationOrder(-1)]
public class PetraSphinxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it doesn't, the player puts it into their graveyard"];
}
