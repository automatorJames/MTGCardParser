namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of March of Burgeoning Life, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>March of Burgeoning Life</exampledoc>
[TokenizationOrder(-1)]
public class MarchOfBurgeoningLifeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target creature with mana value less than x"];
}
