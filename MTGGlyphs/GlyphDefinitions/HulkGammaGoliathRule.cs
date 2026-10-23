namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hulk, Gamma Goliath, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hulk, Gamma Goliath</exampledoc>
[TokenizationOrder(-1)]
public class HulkGammaGoliathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["power-up abilities of other creatures you control cost {3} less to activate"];
}
