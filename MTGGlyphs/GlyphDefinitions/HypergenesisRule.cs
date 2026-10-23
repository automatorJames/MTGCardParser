namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hypergenesis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hypergenesis</exampledoc>
[TokenizationOrder(-1)]
public class HypergenesisRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["starting with you, each player may put an artifact, creature, enchantment, or land card from their hand onto the battlefield"];
}
