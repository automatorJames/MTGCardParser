namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent having (or, as it left, having had, or not having) counters on it: "it has a depletion counter on it", "{this} has a pupa counter on it", "it had no time counters on it", "it had counters on it", "a creature has a -1/-1 counter on it", "vadmir has four or more +1/+1 counters on it" (a legend's short name), "this creature doesn't have a +1/+1 counter on it".</summary>
/// <exampledoc>Land Cap</exampledoc>
/// <examplecapture>it has a depletion counter on it</examplecapture>
[Dependent]
public class HasCounters : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Alt("has", "had", "doesn't have"), Prop(Counters), "on", Prop(Holder)];

    public OneOf<PermanentPhrase, IndefinitePermanent, ShortName> Subject { get; set; }
    public OneOf<Counters, AnyCounters> Counters { get; set; }
    public PermanentPhrase Holder { get; set; }
}
