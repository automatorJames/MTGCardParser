namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"has more life", "has less life": a player's life total, compared.</summary>
/// <exampledoc>Oath of Mages</exampledoc>
/// <examplecapture>has more life</examplecapture>
[Dependent]
public class HasLifeCompared : Glyph
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Comparison), "life"];

    public HaveVerb Verb { get; set; }
    public Comparison Comparison { get; set; }
}
