namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition on a player controlling a permanent of a kind: "unless defending player controls an island", "as long as you control a forest".</summary>
/// <exampledoc>Dandân</exampledoc>
/// <examplecapture>unless defending player controls an island</examplecapture>
[Dependent]
public class ControlCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Connective), Prop(Controller), Pattern("controls?"), Prop(Permanent)];

    public ConditionConnective Connective { get; set; }
    public WhichPlayer Controller { get; set; }
    public IndefinitePermanent Permanent { get; set; }
}
