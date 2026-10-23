namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Edgar, King of Figaro, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Edgar, King of Figaro</exampledoc>
[TokenizationOrder(-1)]
public class EdgarKingOfFigaroRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["two-headed coin — the first time you flip one or more coins each turn, those coins come up heads and you win those flips"];
}
