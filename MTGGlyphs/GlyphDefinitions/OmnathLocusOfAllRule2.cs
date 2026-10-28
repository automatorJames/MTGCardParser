namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Omnath, Locus of All, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Omnath, Locus of All</exampledoc>
[TokenizationOrder(-1)]
public class OmnathLocusOfAllRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, add three mana in any combination of its colors and put it into your hand"];
}
