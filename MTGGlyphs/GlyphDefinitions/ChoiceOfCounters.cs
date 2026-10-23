namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"your choice of a reach counter, a vigilance counter, or a trample counter": one of several kinds of counter, chosen as it's put.</summary>
/// <exampledoc>Flycatcher Giraffid</exampledoc>
/// <examplecapture>your choice of a reach counter or a vigilance counter</examplecapture>
[Dependent]
public class ChoiceOfCounters : Glyph
{
    public override Nib[] Nibs => ["your choice of", Prop(Options)];

    public ManyOf<Counters> Options { get; set; }
}
