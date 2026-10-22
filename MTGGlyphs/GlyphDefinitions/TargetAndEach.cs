namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A target and every permanent of a kind, as one damage recipient: "target player or planeswalker and each creature that player controls", "target creature and each other creature that shares a color with it".</summary>
/// <exampledoc>Flame Wave</exampledoc>
/// <examplecapture>target player or planeswalker and each creature that player or that planeswalker's controller controls</examplecapture>
[Dependent]
public class TargetAndEach : Glyph
{
    public override Nib[] Nibs => [Prop(Target), "and", Prop(Each)];

    public Target Target { get; set; }
    public EachPermanents Each { get; set; }
}
