namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A count, as an amount: "the number of swamps you control", "the number of other attacking creatures", "the number of doom counters on it", "the number of cards in your hand", "the number of basic land types among lands you control", "the number of creatures in your party", "the number of cards revealed this way". The party is tried before plain permanents, which would stop at "creatures". "other" excludes the source itself.</summary>
/// <exampledoc>Nightmare</exampledoc>
/// <examplecapture>the number of swamps you control</examplecapture>
[Dependent]
public class NumberOf : Glyph
{
    public override Nib[] Nibs => ["the number of", Prop(Other), Prop(Counted)];

    [Optional]
    [RegexPattern("other")]
    public bool? Other { get; set; }
    public OneOf<OneOf<PartyCreatures, Permanents>, CountersOn, OneOf<AffectedThisWay, CardsInZone, DistinctKinds>> Counted { get; set; }
}
