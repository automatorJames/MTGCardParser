namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Savage Summoning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Savage Summoning</exampledoc>
[TokenizationOrder(-1)]
public class SavageSummoningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next creature spell you cast this turn can be cast as though it had flash"];
}
