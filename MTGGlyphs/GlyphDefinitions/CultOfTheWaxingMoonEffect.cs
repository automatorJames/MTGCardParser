namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cult of the Waxing Moon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cult of the Waxing Moon</exampledoc>
[Dependent]
public class CultOfTheWaxingMoonEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 2/2 green wolf creature token"];
}
