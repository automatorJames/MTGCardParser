namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lightstall Inquisitor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lightstall Inquisitor</exampledoc>
[TokenizationOrder(-1)]
public class LightstallInquisitorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each land played this way enters tapped"];
}
