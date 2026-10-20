namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Empower's fallback: `if you don't control one, first create a blue jace planeswalker token with "[−1]: surveil 1" and "[−3]: draw a card` - the Jace token created before counters go on it.</summary>
/// <exampledoc>Way of the Mind Sculptor</exampledoc>
/// <examplecapture>if you don't control one, first create a blue jace planeswalker token with "[−1]: surveil 1" and "[−3]: draw a card</examplecapture>
public class EmpowerJaceToken : Glyph
{
    public override Nib[] Nibs => ["if you don't control one, first create a blue jace planeswalker token with \"[−1]: surveil 1\" and \"[−3]: draw a card"];
}
