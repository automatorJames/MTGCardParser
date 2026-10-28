namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Erayo, Soratami Ascendant // Erayo's Essence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Erayo, Soratami Ascendant // Erayo's Essence</exampledoc>
[TokenizationOrder(-1)]
public class ErayoSoratamiAscendantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever the fourth spell of a turn is cast, flip erayo"];
}
