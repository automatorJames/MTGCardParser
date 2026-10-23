namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Coveted Prize, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coveted Prize</exampledoc>
/// <examplecapture>if you have a full party, you may cast a spell with mana value 4 or less from your hand without paying its mana cost</examplecapture>
[TokenizationOrder(-1)]
public class CovetedPrizeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you have a full party, you may cast a spell with mana value 4 or less from your hand without paying its mana cost"];
}
