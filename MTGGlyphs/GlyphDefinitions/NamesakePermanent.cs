namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Any one permanent a name is compared with: "another creature you control", "a nontoken permanent", "another permanent", "a legendary permanent you control". Kept free of with-qualifiers so it can sit inside one (<see cref="WithSameName"/>) without a cycle.</summary>
/// <exampledoc>Pattern Matcher</exampledoc>
/// <examplecapture>another creature you control</examplecapture>
[Dependent]
public class NamesakePermanent : Glyph
{
    public override Nib[] Nibs => [Prop(Determiner), Prop(Qualifier), Prop(Kind), Prop(Controller)];

    public IndefiniteDeterminer Determiner { get; set; }
    [Optional]
    public TargetQualifier Qualifier { get; set; }
    public PermanentKind Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
}
