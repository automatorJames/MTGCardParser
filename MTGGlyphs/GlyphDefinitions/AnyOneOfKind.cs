namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Any one permanent of a kind, with at most one quality: "a zombie", "a blue creature" - kept free of the qualifiers that refer back to other permanent descriptors.</summary>
/// <exampledoc>Time to Reflect</exampledoc>
/// <examplecapture>a zombie</examplecapture>
[Dependent]
public class AnyOneOfKind : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Quality), Prop(Kind)];

    [Optional]
    public TargetQualifier Quality { get; set; }
    public PermanentKind Kind { get; set; }
}
