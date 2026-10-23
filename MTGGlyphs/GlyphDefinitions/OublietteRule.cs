namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Oubliette, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oubliette</exampledoc>
[TokenizationOrder(-1)]
public class OublietteRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap that creature as it phases in this way"];
}
