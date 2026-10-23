namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wood Elemental, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wood Elemental</exampledoc>
[TokenizationOrder(-1)]
public class WoodElementalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "'s power and toughness are each equal to the number of forests sacrificed as it entered"];
}
