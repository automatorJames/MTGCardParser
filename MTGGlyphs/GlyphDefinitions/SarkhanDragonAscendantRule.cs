namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sarkhan, Dragon Ascendant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sarkhan, Dragon Ascendant</exampledoc>
[TokenizationOrder(-1)]
public class SarkhanDragonAscendantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, sarkhan becomes a dragon in addition to his other types and gains flying"];
}
