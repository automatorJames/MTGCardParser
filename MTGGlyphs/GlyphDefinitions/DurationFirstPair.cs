namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two subjects' effects sharing a duration that comes first: "until your next turn, that creature can't attack or block and its activated abilities can't be activated" (detain).</summary>
/// <exampledoc>Azorius Arrester</exampledoc>
/// <examplecapture>until your next turn, that creature can't attack or block and its activated abilities can't be activated</examplecapture>
public class DurationFirstPair : Glyph
{
    public override Nib[] Nibs => [Prop(Duration), ",", Prop(Clauses)];

    public OneOf<UntilPhase, UntilEndOfPlayersTurn> Duration { get; set; }
    public BothSubjectsDo Clauses { get; set; }
}
