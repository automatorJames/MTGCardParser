namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cavernous Maw, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cavernous Maw</exampledoc>
[TokenizationOrder(-1)]
public class CavernousMawRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only if the number of other caves you control plus the number of cave cards in your graveyard is three or greater"];
}
