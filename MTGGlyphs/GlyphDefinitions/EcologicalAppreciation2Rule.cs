namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ecological Appreciation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ecological Appreciation</exampledoc>
[TokenizationOrder(-1)]
public class EcologicalAppreciation2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle the chosen cards into your library and put the rest onto the battlefield"];
}
