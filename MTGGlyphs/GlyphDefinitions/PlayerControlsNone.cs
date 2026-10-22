namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you control no untapped lands", "defending player controls no creatures": a player controlling no permanents of a kind, as a condition.</summary>
/// <exampledoc>Scoria Cat</exampledoc>
/// <examplecapture>you control no untapped lands</examplecapture>
[Dependent]
public class PlayerControlsNone : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Pattern("controls?"), "no", Prop(Permanents)];

    public WhichPlayer Controller { get; set; }
    public Permanents Permanents { get; set; }
}
