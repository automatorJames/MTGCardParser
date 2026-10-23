namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Corruption of Towashi, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Corruption of Towashi</exampledoc>
[Dependent]
public class CorruptionOfTowashiEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may draw a card"];
}
