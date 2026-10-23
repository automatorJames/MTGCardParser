namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chaos Moon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chaos Moon</exampledoc>
/// <examplecapture>if the number is odd, until end of turn, red creatures get +1/+1 and whenever a player taps a mountain for mana, that player adds an additional {r}</examplecapture>
[TokenizationOrder(-1)]
public class ChaosMoon2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the number is odd, until end of turn, red creatures get +1/+1 and whenever a player taps a mountain for mana, that player adds an additional {r}"];
}
