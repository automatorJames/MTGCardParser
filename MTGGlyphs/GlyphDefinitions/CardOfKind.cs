namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a nonland card", "a creature card", "an instant or sorcery card": any one card of a kind, wherever it is.</summary>
/// <exampledoc>Raffine's Informant</exampledoc>
/// <examplecapture>a nonland card</examplecapture>
[Dependent]
public class CardOfKind : Glyph
{
    public override Nib[] Nibs => [Prop(Article), Prop(Qualifiers), "card"];

    public ArticleA Article { get; set; }
    [Optional]
    public OneOf<ManyOf<TargetQualifier>, QualifierRun> Qualifiers { get; set; }
}
