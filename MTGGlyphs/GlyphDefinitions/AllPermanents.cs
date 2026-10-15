namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"all creatures", "all other creatures", "all creatures you control": every permanent of a kind, as a subject or an object.</summary>
/// <exampledoc>Shrieking Mogg</exampledoc>
/// <examplecapture>all other creatures</examplecapture>
[Dependent]
public class AllPermanents : Glyph
{
    public override Nib[] Nibs => ["all", Prop(Other), Prop(Permanents)];

    [RegexPattern("other")]
    public bool Other { get; set; }
    public Permanents Permanents { get; set; }
}
