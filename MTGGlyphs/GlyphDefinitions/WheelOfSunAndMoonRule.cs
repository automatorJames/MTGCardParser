namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wheel of Sun and Moon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wheel of Sun and Moon</exampledoc>
/// <examplecapture>if a card would be put into enchanted player's graveyard from anywhere, instead that card is revealed and put on the bottom of that player's library</examplecapture>
[TokenizationOrder(-1)]
public class WheelOfSunAndMoonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a card would be put into enchanted player's graveyard from anywhere, instead that card is revealed and put on the bottom of that player's library"];
}
