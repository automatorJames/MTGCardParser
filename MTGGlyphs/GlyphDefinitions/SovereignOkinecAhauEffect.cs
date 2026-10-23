namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sovereign Okinec Ahau, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sovereign Okinec Ahau</exampledoc>
/// <examplecapture>for each creature you control with power greater than that creature's base power, put a number of +1/+1 counters on that creature equal to the difference</examplecapture>
[Dependent]
public class SovereignOkinecAhauEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each creature you control with power greater than that creature's base power, put a number of +1/+1 counters on that creature equal to the difference"];
}
