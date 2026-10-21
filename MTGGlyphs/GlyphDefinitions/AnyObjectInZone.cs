namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A clone's choice of what to copy: "any creature on the battlefield", "any artifact or creature on the battlefield", "any nonland permanent on the battlefield", "any creature card in a graveyard". Chosen as the effect happens, not targeted.</summary>
/// <exampledoc>Clone</exampledoc>
/// <examplecapture>any creature on the battlefield</examplecapture>
[Dependent]
public class AnyObjectInZone : Glyph
{
    public override Nib[] Nibs => ["any", Prop(Qualifier), Prop(Kind), Prop(Where)];

    [Optional]
    public OneOf<ManyOf<TargetQualifier>, TargetQualifier, QualifierWords> Qualifier { get; set; }
    public OneOf<ManyOf<PermanentKind>, PermanentKind> Kind { get; set; }
    public CopySourceZone Where { get; set; }
}
