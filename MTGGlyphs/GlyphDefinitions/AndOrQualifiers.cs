namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two qualifiers joined by "and/or": "instant and/or sorcery", "blue and/or black", "bird and/or soldier".</summary>
/// <exampledoc>Calculated Dismissal</exampledoc>
/// <examplecapture>instant and/or sorcery</examplecapture>
[Dependent]
public class AndOrQualifiers : Glyph
{
    public override Nib[] Nibs => [Prop(First), "and/or", Prop(Second)];

    public TargetQualifier First { get; set; }
    public TargetQualifier Second { get; set; }
}
