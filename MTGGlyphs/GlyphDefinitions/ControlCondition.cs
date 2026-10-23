namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition on a player controlling a permanent of a kind, a choice of kinds, or a number of them: "unless defending player controls an island", "as long as you control a forest", "unless you control a swamp or a mountain", "unless you control two or more other lands", "unless you control at least three other creatures", "as long as defending player controls a land of that type", "as long as its controller controls another creature", "as long as that player controls three or fewer lands" (a player named earlier).</summary>
/// <exampledoc>Dandân</exampledoc>
/// <examplecapture>unless defending player controls an island</examplecapture>
[Dependent]
public class ControlCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Connective), Prop(Controller), Pattern("controls?"), Prop(Permanent)];

    public ConditionConnective Connective { get; set; }
    public OneOf<WhichPlayer?, PlayerReference> Controller { get; set; }
    public OneOf<OneOf<PermanentsThreshold, AtLeastPermanents>, ManyOf<IndefinitePermanent>, OneOf<PermanentOfNamedType, IndefinitePermanent>> Permanent { get; set; }
}
