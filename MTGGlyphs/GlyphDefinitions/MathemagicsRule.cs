namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mathemagics, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mathemagics</exampledoc>
[TokenizationOrder(-1)]
public class MathemagicsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player draws 2ˣ cards"];
}
