namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Counterlash, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Counterlash</exampledoc>
[TokenizationOrder(-1)]
public class CounterlashRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a spell that shares a card type with it from your hand without paying its mana cost"];
}
