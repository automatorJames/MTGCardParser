namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition an effect lasts while: a permanent remaining tapped or on the battlefield, a player controlling it, or it having counters on it.</summary>
/// <exampledoc>Ice Floe</exampledoc>
/// <examplecapture>{this} remains tapped</examplecapture>
[Dependent]
public class LastingCondition : GlyphOneOf
{
    public Remains Remains { get; set; }
    public ControlsPermanent ControlsPermanent { get; set; }
    public HasCounters HasCounters { get; set; }
}
