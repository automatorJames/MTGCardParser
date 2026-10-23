namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword action performed by its subject, as a predicate: "it explores", "target creature you control explores", "it connives", "it endures 2", "it explores again", and players' actions, "target player scries 3", "its controller investigates", "its controller manifests dread", "that player discovers x".</summary>
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
