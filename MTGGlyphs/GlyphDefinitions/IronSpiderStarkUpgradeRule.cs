namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Iron Spider, Stark Upgrade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Iron Spider, Stark Upgrade</exampledoc>
[TokenizationOrder(-1)]
public class IronSpiderStarkUpgradeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{2}, remove two +1/+1 counters from among artifacts you control: draw a card"];
}
