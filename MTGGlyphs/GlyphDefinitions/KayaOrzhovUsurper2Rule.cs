namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kaya, Orzhov Usurper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaya, Orzhov Usurper</exampledoc>
[TokenizationOrder(-1)]
public class KayaOrzhovUsurper2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain 2 life if at least one creature card was exiled this way"];
}
