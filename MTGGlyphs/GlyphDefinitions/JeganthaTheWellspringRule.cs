namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jegantha, the Wellspring, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jegantha, the Wellspring</exampledoc>
[TokenizationOrder(-1)]
public class JeganthaTheWellspringRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — no card in your starting deck has more than one of the same mana symbol in its mana cost"];
}
