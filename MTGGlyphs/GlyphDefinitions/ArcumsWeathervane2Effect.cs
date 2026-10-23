namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Arcum's Weathervane, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Arcum's Weathervane</exampledoc>
[Dependent]
public class ArcumsWeathervane2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target nonsnow basic land becomes snow"];
}
