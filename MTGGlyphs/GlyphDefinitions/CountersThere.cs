namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Counters being on a permanent, as a condition: "there are no charge counters on {this}".</summary>
/// <exampledoc>Jeweled Amulet</exampledoc>
/// <examplecapture>there are no charge counters on {this}</examplecapture>
[Dependent]
public class CountersThere : Glyph
{
    public override Nib[] Nibs => [Pattern("there (are|is)"), Prop(Counters), "on", Prop(Holder)];

    public Counters Counters { get; set; }
    public PermanentPhrase Holder { get; set; }
}
