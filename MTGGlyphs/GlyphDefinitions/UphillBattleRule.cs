namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Uphill Battle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Uphill Battle</exampledoc>
[TokenizationOrder(-1)]
public class UphillBattleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures played by your opponents enter tapped"];
}
