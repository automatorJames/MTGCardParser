namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Invert // Invent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invert // Invent</exampledoc>
[TokenizationOrder(-1)]
public class InvertRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["switch the power and toughness of each of up to two target creatures until end of turn"];
}
