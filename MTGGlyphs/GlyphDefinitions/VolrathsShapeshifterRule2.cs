namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Volrath's Shapeshifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Volrath's Shapeshifter</exampledoc>
[TokenizationOrder(-1)]
public class VolrathsShapeshifterRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(this creature has that card's name, mana cost, color, types, abilities, power, and toughness"];
}
