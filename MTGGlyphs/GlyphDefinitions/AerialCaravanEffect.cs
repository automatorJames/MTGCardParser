namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Aerial Caravan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aerial Caravan</exampledoc>
[Dependent]
public class AerialCaravanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reveal the card as you exile it"];
}
