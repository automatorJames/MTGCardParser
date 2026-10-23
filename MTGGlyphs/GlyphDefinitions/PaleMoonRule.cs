namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pale Moon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pale Moon</exampledoc>
[TokenizationOrder(-1)]
public class PaleMoonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, if a player taps a nonbasic land for mana, it produces colorless mana instead of any other type"];
}
