namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Peter Parker's Camera, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Peter Parker's Camera</exampledoc>
[TokenizationOrder(-1)]
public class PeterParkersCameraCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{2}, {t}, remove a film counter from this artifact: copy target activated or triggered ability you control"];
}
