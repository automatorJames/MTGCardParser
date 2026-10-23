namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Charmed Pendant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Charmed Pendant</exampledoc>
[TokenizationOrder(-1)]
public class CharmedPendantCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for example, if the card's mana cost is {3}{u}{u}{b}, you add {u}{u}{b}"];
}
