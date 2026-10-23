namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"creature tapped this way", "permanent sacrificed this way", "card revealed this way", "creature card exiled this way", "nontoken creature exiled this way", "cards revealed this way", "card exiled from their hand this way", "card of the chosen type revealed this way", "card with the chosen name revealed this way": each object the effect just described acted on, singular (after "for each") or plural (after "the number of"), optionally by a choice made earlier and by the zone it left.</summary>
/// <exampledoc>Ledev Champion</exampledoc>
/// <examplecapture>creature tapped this way</examplecapture>
[Dependent]
public class AffectedThisWay : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural(Prop(Kind)), Prop(Card), Prop(Chosen), Prop(Action), Prop(From), "this way"];

    [Optional]
    public QualifierWords Qualifiers { get; set; }
    public OneOf<PermanentKind, CardNoun?> Kind { get; set; }
    [Optional]
    [RegexPattern("cards?")]
    public bool? Card { get; set; }
    [Optional]
    public OneOf<WithChosenName, ChosenAspect?> Chosen { get; set; }
    public ThisWayAction Action { get; set; }
    [Optional]
    public FromZone From { get; set; }
}
