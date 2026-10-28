namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A target's possessive: "target creature's", "target player's", "target opponent's". Apart from ObjectPossessive because a target's controller can itself be named by a possessive.</summary>
/// <exampledoc>About Face</exampledoc>
/// <examplecapture>target creature's</examplecapture>
[Dependent]
public class TargetPossessive : Glyph
{
    public override Nib[] Nibs => [Prop(Target), "'s"];

    public SpecificTarget Target { get; set; }
}
