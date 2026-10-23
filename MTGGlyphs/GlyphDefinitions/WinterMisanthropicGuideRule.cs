namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Winter, Misanthropic Guide, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winter, Misanthropic Guide</exampledoc>
/// <examplecapture>delirium — as long as there are four or more card types among cards in your graveyard, each opponent's maximum hand size is equal to seven minus the number of those card types</examplecapture>
[TokenizationOrder(-1)]
public class WinterMisanthropicGuideRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["delirium — as long as there are four or more card types among cards in your graveyard, each opponent's maximum hand size is equal to seven minus the number of those card types"];
}
