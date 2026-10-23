namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Iron Spider, Stark Upgrade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Iron Spider, Stark Upgrade</exampledoc>
[Dependent]
public class IronSpiderStarkUpgradeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}
