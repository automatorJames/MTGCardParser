namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Time Stretch, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Time Stretch</exampledoc>
[TokenizationOrder(-1)]
public class TimeStretchRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player takes two extra turns after this one"];
}
