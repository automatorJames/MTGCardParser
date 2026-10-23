namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shapeshifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shapeshifter</exampledoc>
[TokenizationOrder(-1)]
public class ShapeshifterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "'s power is equal to the last chosen number and its toughness is equal to 7 minus that number"];
}
