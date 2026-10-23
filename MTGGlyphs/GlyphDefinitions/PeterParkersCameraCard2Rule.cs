namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Peter Parker's Camera, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Peter Parker's Camera</exampledoc>
[TokenizationOrder(-1)]
public class PeterParkersCameraCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this artifact enters with three film counters on it"];
}
