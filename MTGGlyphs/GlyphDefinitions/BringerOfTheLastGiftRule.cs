namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bringer of the Last Gift, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bringer of the Last Gift</exampledoc>
[TokenizationOrder(-1)]
public class BringerOfTheLastGiftRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then each player returns all creature cards from their graveyard that weren't put there this way to the battlefield"];
}
