namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cursecloth Wrappings, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cursecloth Wrappings</exampledoc>
[TokenizationOrder(-1)]
public class CurseclothWrappingsCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(exile that card and pay its embalm cost: create a token that's a copy of it, except it's a white zombie in addition to its other types and has no mana cost"];
}
