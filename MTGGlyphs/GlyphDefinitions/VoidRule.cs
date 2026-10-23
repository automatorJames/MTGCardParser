namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Void, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Void</exampledoc>
[TokenizationOrder(-1)]
public class VoidRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then target player reveals their hand and discards all nonland cards with mana value equal to the number"];
}
