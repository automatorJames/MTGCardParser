namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chandra, Pyromaster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra, Pyromaster</exampledoc>
[TokenizationOrder(-1)]
public class ChandraPyromasterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose an instant or sorcery card exiled this way and copy it three times"];
}
