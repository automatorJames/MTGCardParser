namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bartered Cow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bartered Cow</exampledoc>
[TokenizationOrder(-1)]
public class BarteredCowCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this creature dies and when you discard this card, create a food token"];
}
