namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Guided Passage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guided Passage</exampledoc>
[TokenizationOrder(-1)]
public class GuidedPassageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reveal the cards in your library"];
}
