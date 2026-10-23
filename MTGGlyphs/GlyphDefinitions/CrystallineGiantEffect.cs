namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Crystalline Giant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crystalline Giant</exampledoc>
/// <examplecapture>choose a kind of counter at random that this creature doesn't have on it from among flying, first strike, deathtouch, hexproof, lifelink, menace, reach, trample, vigilance, and +1/+1</examplecapture>
[Dependent]
public class CrystallineGiantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a kind of counter at random that this creature doesn't have on it from among flying, first strike, deathtouch, hexproof, lifelink, menace, reach, trample, vigilance, and +1/+1"];
}
