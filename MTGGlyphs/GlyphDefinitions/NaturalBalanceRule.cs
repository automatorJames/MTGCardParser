namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Natural Balance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Natural Balance</exampledoc>
[TokenizationOrder(-1)]
public class NaturalBalanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player who controls six or more lands chooses five lands they control and sacrifices the rest"];
}
