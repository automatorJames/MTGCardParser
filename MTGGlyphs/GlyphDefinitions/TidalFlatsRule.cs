namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tidal Flats, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tidal Flats</exampledoc>
[TokenizationOrder(-1)]
public class TidalFlatsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that player doesn't, creatures you control blocking that creature gain first strike until end of turn"];
}
