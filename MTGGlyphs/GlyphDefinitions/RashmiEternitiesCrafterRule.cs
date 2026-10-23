namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rashmi, Eternities Crafter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rashmi, Eternities Crafter</exampledoc>
[TokenizationOrder(-1)]
public class RashmiEternitiesCrafterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast it without paying its mana cost if it's a spell with lesser mana value"];
}
