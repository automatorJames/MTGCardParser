namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect delayed to a later point in the turn: "destroy {this} at the beginning of the next end step", "destroy that creature at the beginning of the next end step".</summary>
/// <exampledoc>Rocket Launcher</exampledoc>
/// <examplecapture>destroy {this} at the beginning of the next end step</examplecapture>
public class DelayedEffect : Glyph
{
    public override Nib[] Nibs => [Prop(Effect), Prop(When)];

    public DynamicGlyph Effect { get; set; }
    public AtOrUntilPlayerPhase When { get; set; }
}
