namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"for each other creature you control that the spell could target", "for each other permanent or player the spell could target", "for each other golem that spell could target": one copy per other object the spell could legally target.</summary>
/// <exampledoc>Precursor Golem</exampledoc>
/// <examplecapture>for each other golem that spell could target</examplecapture>
[Dependent]
public class ForEachOtherTarget : Glyph
{
    public override Nib[] Nibs => ["for each other", Prop(Kinds), Prop(Controller), Alt("that the spell could target", "the spell could target", "that spell could target")];

    public OneOf<ManyOf<TargetKind>, TargetKind> Kinds { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
}
