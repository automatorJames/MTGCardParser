namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Seafarer's Quay, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Seafarer's Quay</exampledoc>
[TokenizationOrder(-1)]
public class SeafarersQuayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["blue legendary creatures you control have \"bands with other legendary creatures"];
}
