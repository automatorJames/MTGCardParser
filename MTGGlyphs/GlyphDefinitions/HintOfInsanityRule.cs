namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hint of Insanity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hint of Insanity</exampledoc>
[TokenizationOrder(-1)]
public class HintOfInsanityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player discards all nonland cards with the same name as another card in their hand"];
}
