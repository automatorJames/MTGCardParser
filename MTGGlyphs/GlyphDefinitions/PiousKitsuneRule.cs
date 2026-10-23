namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pious Kitsune, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pious Kitsune</exampledoc>
[TokenizationOrder(-1)]
public class PiousKitsuneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if a creature named eight-and-a-half-tails is on the battlefield, you gain 1 life for each devotion counter on this creature"];
}
