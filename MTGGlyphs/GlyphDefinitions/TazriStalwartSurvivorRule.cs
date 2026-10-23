namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tazri, Stalwart Survivor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tazri, Stalwart Survivor</exampledoc>
[TokenizationOrder(-1)]
public class TazriStalwartSurvivorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put all creature cards with activated abilities that aren't mana abilities from among the milled cards into your hand"];
}
