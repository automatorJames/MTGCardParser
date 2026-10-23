namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Aladdin's Lamp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aladdin's Lamp</exampledoc>
/// <examplecapture>the next time you would draw a card this turn, instead look at the top x cards of your library, put all but one of them on the bottom of your library in a random order, then draw a card</examplecapture>
[Dependent]
public class AladdinsLampEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time you would draw a card this turn, instead look at the top x cards of your library, put all but one of them on the bottom of your library in a random order, then draw a card"];
}
