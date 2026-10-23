namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jandor's Ring, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jandor's Ring</exampledoc>
[Dependent]
public class JandorsRingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}
