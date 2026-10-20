namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Speed's reminder text (after "start your engines!"), first sentence.</summary>
/// <exampledoc>Racers' Scoreboard</exampledoc>
/// <examplecapture>if you have no speed, it starts at 1</examplecapture>
public class SpeedRule : Glyph
{
    public override Nib[] Nibs => ["if you have no speed, it starts at 1"];
}
