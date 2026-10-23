namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Braid of Fire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Braid of Fire</exampledoc>
[TokenizationOrder(-1)]
public class BraidOfFireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cumulative upkeep—add {r}"];
}
