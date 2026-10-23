namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Happily Ever After, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Happily Ever After</exampledoc>
/// <examplecapture>if there are five colors among permanents you control, there are six or more card types among permanents you control and/or cards in your graveyard, and your life total is greater than or equal to your starting life total, you win the game</examplecapture>
[Dependent]
public class HappilyEverAfterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if there are five colors among permanents you control, there are six or more card types among permanents you control and/or cards in your graveyard, and your life total is greater than or equal to your starting life total, you win the game"];
}
