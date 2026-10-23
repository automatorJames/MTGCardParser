namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mystic Subdual, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mystic Subdual</exampledoc>
[TokenizationOrder(-1)]
public class MysticSubdual2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it can gain abilities in other ways"];
}
