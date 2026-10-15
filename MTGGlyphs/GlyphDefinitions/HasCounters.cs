namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent having counters on it: "it has a depletion counter on it", "{this} has a pupa counter on it".</summary>
/// <exampledoc>Land Cap</exampledoc>
/// <examplecapture>it has a depletion counter on it</examplecapture>
[Dependent]
public class HasCounters : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), "has", Prop(Counters), "on", Prop(Holder)];

    public PermanentPhrase Subject { get; set; }
    public Counters Counters { get; set; }
    public PermanentPhrase Holder { get; set; }
}
