namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ertai's Meddling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ertai's Meddling</exampledoc>
[TokenizationOrder(-1)]
public class ErtaisMeddlingCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target spell's controller exiles it with x delay counters on it"];
}
