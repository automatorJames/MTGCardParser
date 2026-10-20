namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword action a creature performs, as a predicate after its subject: "it explores", "target creature you control explores", "it connives".</summary>
/// <exampledoc>Dire Fleet Interloper</exampledoc>
/// <examplecapture>explores</examplecapture>
[Dependent]
public class PerformsKeywordAction : Glyph, IPredicate
{
    public CreatureKeywordAction Action { get; set; }
}
