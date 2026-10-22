namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The revealed cards of some kind: "all creature cards revealed this way", "all goblin cards revealed this way", "all creature and land cards revealed this way".</summary>
/// <exampledoc>Goblin Ringleader</exampledoc>
/// <examplecapture>all goblin cards revealed this way</examplecapture>
[Dependent]
public class CardsRevealedThisWay : Glyph
{
    public override Nib[] Nibs => ["all", Prop(Qualifiers), Plural("card"), "revealed this way"];

    public QualifierWords Qualifiers { get; set; }
}
