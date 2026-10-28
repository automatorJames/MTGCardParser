namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards an earlier effect in the line handled, named by how: "the cards milled this way", "the nonland cards exiled this way", "the other cards exiled this way".</summary>
/// <exampledoc>Wasteful Harvest</exampledoc>
/// <examplecapture>the cards milled this way</examplecapture>
[Dependent]
public class CardsHandledThisWay : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Other), Prop(Qualifiers), Plural("card"), Prop(How), "this way"];

    [Optional]
    [RegexPattern("other")]
    public bool? Other { get; set; }
    [Optional]
    public QualifierWords Qualifiers { get; set; }
    public ThisWayAction How { get; set; }
}
