namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards of a kind that an earlier effect in the line handled, counted: "all creature cards revealed this way", "all cards of the chosen type revealed this way", "up to one enchantment card milled this way", "a land card milled this way", "each goblin card milled this way", "up to one land card discarded this way".</summary>
/// <exampledoc>Goblin Ringleader</exampledoc>
/// <examplecapture>all goblin cards revealed this way</examplecapture>
[Dependent]
public class CardsRevealedThisWay : Glyph
{
    public override Nib[] Nibs => [Prop(Count), Prop(Qualifiers), Plural("card"), Prop(Bound), Prop(How), "this way"];

    public OneOf<UpToCount, Quantity?, EveryOneOf?> Count { get; set; }
    [Optional]
    public QualifierWords Qualifiers { get; set; }
    [Optional]
    public OneOf<ManaValueLimit, WithQualifier, ChosenAspect?> Bound { get; set; }
    public ThisWayAction How { get; set; }
}
