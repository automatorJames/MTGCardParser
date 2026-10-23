namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Full Bore, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Full Bore</exampledoc>
[TokenizationOrder(-1)]
public class FullBoreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that creature was cast for its warp cost, it also gains trample and haste until end of turn"];
}
