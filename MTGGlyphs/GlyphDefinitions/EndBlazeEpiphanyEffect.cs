namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of End-Blaze Epiphany, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>End-Blaze Epiphany</exampledoc>
/// <examplecapture>exile a number of cards from the top of your library equal to its power, then choose a card exiled this way</examplecapture>
[Dependent]
public class EndBlazeEpiphanyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile a number of cards from the top of your library equal to its power, then choose a card exiled this way"];
}
