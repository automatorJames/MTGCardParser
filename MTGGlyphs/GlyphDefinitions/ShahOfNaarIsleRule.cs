namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shah of Naar Isle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shah of Naar Isle</exampledoc>
[TokenizationOrder(-1)]
public class ShahOfNaarIsleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this creature's echo cost is paid, each opponent may draw up to three cards"];
}
