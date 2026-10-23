namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Stature, Size Shifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stature, Size Shifter</exampledoc>
[TokenizationOrder(-1)]
public class StatureSizeShifterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["stature can't be blocked if her power is 1 or less"];
}
