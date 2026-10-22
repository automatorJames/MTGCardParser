namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger-doubling static ability (Panharmonicon, Teysa Karlov): "if a permanent entering causes a triggered ability of a permanent you control to trigger, that ability triggers an additional time".</summary>
/// <exampledoc>Panharmonicon</exampledoc>
/// <examplecapture>if an artifact or creature entering causes a triggered ability of a permanent you control to trigger, that ability triggers an additional time</examplecapture>
public class AdditionalTrigger : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Cause), Prop(Event), "causes a triggered ability of", Prop(Source), "to trigger, that ability triggers an additional time"];

    public IndefinitePermanent Cause { get; set; }
    public TriggeringEvent Event { get; set; }
    public IndefinitePermanent Source { get; set; }
}
