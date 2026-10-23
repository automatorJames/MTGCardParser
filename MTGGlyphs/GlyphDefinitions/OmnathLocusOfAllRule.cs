namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Omnath, Locus of All, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Omnath, Locus of All</exampledoc>
[TokenizationOrder(-1)]
public class OmnathLocusOfAllRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may reveal that card if it has three or more colored mana symbols in its mana cost"];
}
