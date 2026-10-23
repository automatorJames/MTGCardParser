namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rakdos, the Showstopper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rakdos, the Showstopper</exampledoc>
[Dependent]
public class RakdosTheShowstopperEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip a coin for each creature that isn't a demon, devil, or imp"];
}
