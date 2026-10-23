namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Niv-Mizzet, Visionary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Niv-Mizzet, Visionary</exampledoc>
[Dependent]
public class NivMizzetVisionaryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you draw that many cards"];
}
