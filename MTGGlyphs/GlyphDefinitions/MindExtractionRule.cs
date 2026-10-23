namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mind Extraction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mind Extraction</exampledoc>
[TokenizationOrder(-1)]
public class MindExtractionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player reveals their hand and discards all cards of each of the sacrificed creature's colors"];
}
