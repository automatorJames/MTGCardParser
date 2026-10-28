namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cult of the Waxing Moon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cult of the Waxing Moon</exampledoc>
[TokenizationOrder(-1)]
public class CultOfTheWaxingMoonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a permanent you control transforms into a non-human creature, create a 2/2 green wolf creature token"];
}
