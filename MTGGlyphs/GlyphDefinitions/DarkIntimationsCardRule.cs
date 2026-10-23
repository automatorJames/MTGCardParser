namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dark Intimations, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dark Intimations</exampledoc>
[TokenizationOrder(-1)]
public class DarkIntimationsCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you cast a bolas planeswalker spell, exile this card from your graveyard"];
}
