namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"each of two target creatures", "each of up to two target creatures": several targets, each affected in full.</summary>
/// <exampledoc>Swelter</exampledoc>
/// <examplecapture>each of two target creatures</examplecapture>
[Dependent]
public class EachOfTargets : Glyph
{
    public override Nib[] Nibs => ["each of", Prop(Targets)];

    public CountedTargets Targets { get; set; }
}
