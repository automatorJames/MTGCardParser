namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Long-Term Plans, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Long-Term Plans</exampledoc>
[TokenizationOrder(-1)]
public class LongTermPlansRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for a card, then shuffle and put that card third from the top"];
}
