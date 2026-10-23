namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Darkness Crystal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Darkness Crystal</exampledoc>
[Dependent]
public class TheDarknessCrystalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put target creature card exiled with", Nib.This, "onto the battlefield tapped under your control with two additional +1/+1 counters on it"];
}
