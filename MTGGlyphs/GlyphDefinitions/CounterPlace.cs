namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where counters go or come from: "on target creature", "from {this}".</summary>
/// <exampledoc>Armageddon Clock</exampledoc>
/// <examplecapture>on {this}</examplecapture>
[Dependent]
public class CounterPlace : Glyph
{
    public override Nib[] Nibs => [Alt("on", "from"), Prop(Permanent)];

    public CounterRecipient Permanent { get; set; }
}
