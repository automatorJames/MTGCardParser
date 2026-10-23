namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glyph of Reincarnation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glyph of Reincarnation</exampledoc>
[TokenizationOrder(-1)]
public class GlyphOfReincarnationCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures that were blocked by target wall this turn"];
}
