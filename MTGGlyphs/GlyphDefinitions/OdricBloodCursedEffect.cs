namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Odric, Blood-Cursed, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Odric, Blood-Cursed</exampledoc>
/// <examplecapture>create x blood tokens, where x is the number of abilities from among flying, first strike, double strike, deathtouch, haste, hexproof, indestructible, lifelink, menace, reach, trample, and vigilance found among creatures you control</examplecapture>
[Dependent]
public class OdricBloodCursedEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create x blood tokens, where x is the number of abilities from among flying, first strike, double strike, deathtouch, haste, hexproof, indestructible, lifelink, menace, reach, trample, and vigilance found among creatures you control"];
}
