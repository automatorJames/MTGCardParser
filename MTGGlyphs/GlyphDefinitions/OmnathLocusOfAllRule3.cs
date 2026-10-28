namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Omnath, Locus of All, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Omnath, Locus of All</exampledoc>
[TokenizationOrder(-1)]
public class OmnathLocusOfAllRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you don't reveal it, put it into your hand"];
}
