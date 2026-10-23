namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Niv-Mizzet Reborn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Niv-Mizzet Reborn</exampledoc>
[TokenizationOrder(-1)]
public class NivMizzetRebornRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each color pair, choose a card that's exactly those colors from among them"];
}
