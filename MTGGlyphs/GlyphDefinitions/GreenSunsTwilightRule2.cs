namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Green Sun's Twilight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Green Sun's Twilight</exampledoc>
/// <examplecapture>if x is 5 or more, instead put the chosen cards onto the battlefield or into your hand and the rest on the bottom of your library in a random order</examplecapture>
[TokenizationOrder(-1)]
public class GreenSunsTwilightRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if x is 5 or more, instead put the chosen cards onto the battlefield or into your hand and the rest on the bottom of your library in a random order"];
}
