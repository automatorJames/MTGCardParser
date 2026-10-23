namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Erayo, Soratami Ascendant // Erayo's Essence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Erayo, Soratami Ascendant // Erayo's Essence</exampledoc>
[Dependent]
public class ErayoSoratamiAscendantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip erayo"];
}
