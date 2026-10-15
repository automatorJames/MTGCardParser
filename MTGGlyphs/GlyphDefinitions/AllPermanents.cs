namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"all creatures", "all walls", "all creatures you control": every permanent of a kind, as a subject.</summary>
/// <exampledoc>Whiteout</exampledoc>
/// <examplecapture>all creatures</examplecapture>
[Dependent]
public class AllPermanents : Glyph
{
    public override Nib[] Nibs => ["all", Prop(Permanents)];

    public Permanents Permanents { get; set; }
}
