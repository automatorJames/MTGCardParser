namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Roots of Wisdom, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Roots of Wisdom</exampledoc>
[TokenizationOrder(-1)]
public class RootsOfWisdomRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mill three cards, then return a land card or elf card from your graveyard to your hand"];
}
