namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Omnath, Locus of Mana, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Omnath, Locus of Mana</exampledoc>
[TokenizationOrder(-1)]
public class OmnathLocusOfManaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["omnath gets +1/+1 for each unspent green mana you have"];
}
