namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lagrella, the Magpie, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lagrella, the Magpie</exampledoc>
[Dependent]
public class LagrellaTheMagpieEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put two +1/+1 counters on it"];
}
