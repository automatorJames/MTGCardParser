namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cathedral of Serra, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cathedral of Serra</exampledoc>
[TokenizationOrder(-1)]
public class CathedralOfSerraRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["white legendary creatures you control have \"bands with other legendary creatures"];
}
