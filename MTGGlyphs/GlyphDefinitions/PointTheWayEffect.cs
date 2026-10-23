namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Point the Way, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Point the Way</exampledoc>
[Dependent]
public class PointTheWayEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for up to x basic land cards, where x is your speed"];
}
