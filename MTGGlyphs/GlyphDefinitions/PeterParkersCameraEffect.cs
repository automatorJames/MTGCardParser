namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Peter Parker's Camera, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Peter Parker's Camera</exampledoc>
[Dependent]
public class PeterParkersCameraEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy target activated or triggered ability you control"];
}
