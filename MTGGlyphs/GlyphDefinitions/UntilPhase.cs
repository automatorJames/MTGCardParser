namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How long an effect lasts: "until end of turn".</summary>
/// <exampledoc>Coral Helm</exampledoc>
/// <examplecapture>until end of turn</examplecapture>
[Dependent]
public class UntilPhase : Glyph
{
    public override Nib[] Nibs => ["until", Prop(Phase)];

    public Phase Phase { get; set; }
}
