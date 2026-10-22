namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A landwalk keyword narrowed by a qualifier on the land: "snow swampwalk", "nonbasic landwalk", "legendary landwalk".</summary>
/// <exampledoc>Legions of Lim-Dûl</exampledoc>
/// <examplecapture>snow swampwalk</examplecapture>
[Dependent]
public class QualifiedLandwalk : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifier), Prop(Keyword)];

    public TargetQualifier Qualifier { get; set; }
    public Keyword Keyword { get; set; }
}
