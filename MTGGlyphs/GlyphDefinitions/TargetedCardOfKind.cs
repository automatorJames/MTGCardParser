namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"up to one target instant card", "target creature card": one targeted card of a kind, in a list of them sharing one zone.</summary>
/// <exampledoc>Pull from the Deep</exampledoc>
/// <examplecapture>up to one target instant card</examplecapture>
[Dependent]
[Referent]
public class TargetedCardOfKind : Glyph
{
    public override Nib[] Nibs => [Prop(UpTo), "target", Prop(Qualifiers), "card"];

    [Optional]
    public UpToCount UpTo { get; set; }
    [Optional]
    public OneOf<CompoundOf<TargetQualifier>, QualifierWords> Qualifiers { get; set; }
}
