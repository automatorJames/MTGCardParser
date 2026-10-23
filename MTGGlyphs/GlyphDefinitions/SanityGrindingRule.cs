namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sanity Grinding, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sanity Grinding</exampledoc>
[TokenizationOrder(-1)]
public class SanityGrindingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each blue mana symbol in the mana costs of the revealed cards, target opponent mills a card"];
}
