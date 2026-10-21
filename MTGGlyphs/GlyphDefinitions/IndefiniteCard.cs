namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Some card, not targeted and in no particular zone: "a card", "an instant or sorcery card", "a card or token" - the subject of a replacement effect's "would".</summary>
/// <exampledoc>Rest in Peace</exampledoc>
/// <examplecapture>a card or token</examplecapture>
[Dependent]
public class IndefiniteCard : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Qualifiers), "card", Prop(OrToken)];

    [Optional]
    public OneOf<ManyOf<TargetQualifier>, QualifierRun, AndOrQualifiers> Qualifiers { get; set; }
    [RegexPattern("or token")]
    public bool OrToken { get; set; }
}
