namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Agonizing Remorse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Agonizing Remorse</exampledoc>
[TokenizationOrder(-1)]
public class AgonizingRemorseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you choose a nonland card from it or a card from their graveyard"];
}
