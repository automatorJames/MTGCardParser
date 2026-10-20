namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Riot: "with your choice of a +1/+1 counter or haste".</summary>
/// <exampledoc>Rampaging Rendhorn</exampledoc>
/// <examplecapture>with your choice of a +1/+1 counter or haste</examplecapture>
[Dependent]
public class RiotChoice : Glyph
{
    public override Nib[] Nibs => ["with your choice of", Prop(Counter), "or", Prop(Keyword)];

    public Counters Counter { get; set; }
    public Keyword Keyword { get; set; }
}
