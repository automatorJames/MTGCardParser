namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword action or effect done repeatedly: "investigate three times", "it explores x times", "incubate 2 twice", "double target creature's power x times". The count reuses <see cref="CopyRepetition"/>.</summary>
/// <exampledoc>Confirm Suspicions</exampledoc>
/// <examplecapture>investigate three times</examplecapture>
public class RepeatedAction : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), Prop(Times)];

    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Action { get; set; }
    public CopyRepetition Times { get; set; }
}
