namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Viv Vision, Teen Synthezoid, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Viv Vision, Teen Synthezoid</exampledoc>
[Dependent]
public class VivVisionTeenSynthezoidEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card if her power is 4 or greater"];
}
