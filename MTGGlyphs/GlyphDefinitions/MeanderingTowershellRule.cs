namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Meandering Towershell, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Meandering Towershell</exampledoc>
[TokenizationOrder(-1)]
public class MeanderingTowershellRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return it to the battlefield under your control tapped and attacking at the beginning of the declare attackers step on your next turn"];
}
