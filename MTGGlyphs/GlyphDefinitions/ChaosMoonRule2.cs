namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chaos Moon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chaos Moon</exampledoc>
/// <examplecapture>if the number is even, until end of turn, red creatures get -1/-1 and if a player taps a mountain for mana, that mountain produces colorless mana instead of any other type</examplecapture>
[TokenizationOrder(-1)]
public class ChaosMoonRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the number is even, until end of turn, red creatures get -1/-1 and if a player taps a mountain for mana, that mountain produces colorless mana instead of any other type"];
}
