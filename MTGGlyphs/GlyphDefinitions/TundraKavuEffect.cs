namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tundra Kavu, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tundra Kavu</exampledoc>
[Dependent]
public class TundraKavuEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target land becomes a plains or an island until end of turn"];
}
