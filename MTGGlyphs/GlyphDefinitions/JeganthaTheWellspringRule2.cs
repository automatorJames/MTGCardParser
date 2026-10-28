namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jegantha, the Wellspring, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jegantha, the Wellspring</exampledoc>
[TokenizationOrder(-1)]
public class JeganthaTheWellspringRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this mana can't be spent to pay generic mana costs"];
}
