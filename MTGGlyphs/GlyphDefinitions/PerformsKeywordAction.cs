namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword action a creature performs, as a predicate after its subject: "it explores", "target creature you control explores", "it connives", "it endures 2", "it explores again".</summary>
/// <exampledoc>Dire Fleet Interloper</exampledoc>
/// <examplecapture>explores</examplecapture>
[Dependent]
public class PerformsKeywordAction : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), Prop(Amount), Prop(Again)];

    public CreatureKeywordAction Action { get; set; }
    [Optional]
    public Quantity? Amount { get; set; }
    [Optional]
    [RegexPattern("again")]
    public bool Again { get; set; }
}
