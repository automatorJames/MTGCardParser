namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Songcrafter Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Songcrafter Mage</exampledoc>
[TokenizationOrder(-1)]
public class SongcrafterMageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its harmonize cost is equal to its mana cost"];
}
