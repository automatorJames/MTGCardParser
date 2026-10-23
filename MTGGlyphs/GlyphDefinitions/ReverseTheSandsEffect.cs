namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Reverse the Sands, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reverse the Sands</exampledoc>
[Dependent]
public class ReverseTheSandsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each of those players gets one life total back"];
}
