namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Transcendent Archaic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Transcendent Archaic</exampledoc>
[TokenizationOrder(-1)]
public class TranscendentArchaicRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you draw one or more cards this way, discard two cards"];
}
