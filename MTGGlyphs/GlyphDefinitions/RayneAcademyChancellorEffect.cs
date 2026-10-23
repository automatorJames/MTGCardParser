namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rayne, Academy Chancellor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rayne, Academy Chancellor</exampledoc>
[Dependent]
public class RayneAcademyChancellorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may draw a card"];
}
