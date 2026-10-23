namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Skyship Weatherlight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skyship Weatherlight</exampledoc>
[Dependent]
public class SkyshipWeatherlightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a card at random that was exiled with", Nib.This];
}
