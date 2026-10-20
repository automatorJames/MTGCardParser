namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition on a player controlling a permanent of a kind, a choice of kinds, or a number of them: "unless defending player controls an island", "as long as you control a forest", "unless you control a swamp or a mountain", "unless you control two or more other lands".</summary>
/// <exampledoc>Dandân</exampledoc>
/// <examplecapture>unless defending player controls an island</examplecapture>
[Dependent]
public class ControlCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Connective), Prop(Controller), Pattern("controls?"), Prop(Permanent)];

    public ConditionConnective Connective { get; set; }
    public WhichPlayer Controller { get; set; }
    public OneOf<PermanentsThreshold, ManyOf<IndefinitePermanent>, IndefinitePermanent> Permanent { get; set; }
}
