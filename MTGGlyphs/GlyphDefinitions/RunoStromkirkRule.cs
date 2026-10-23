namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Runo Stromkirk // Krothuss, Lord of the Deep, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Runo Stromkirk // Krothuss, Lord of the Deep</exampledoc>
[TokenizationOrder(-1)]
public class RunoStromkirkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature card with mana value 6 or greater is revealed this way, transform runo"];
}
