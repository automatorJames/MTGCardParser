namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Contract from Below, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Contract from Below</exampledoc>
[TokenizationOrder(-1)]
public class ContractFromBelowRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discard your hand, ante the top card of your library, then draw seven cards"];
}
