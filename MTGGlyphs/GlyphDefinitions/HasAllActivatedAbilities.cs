namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Gaining every activated ability of other objects: "has all activated abilities of all creature cards exiled with it", "gains all activated abilities of target creature until end of turn", "has all activated abilities of all creatures your opponents control", "has all activated abilities of all elf cards in your graveyard".</summary>
/// <exampledoc>Myr Welder</exampledoc>
/// <examplecapture>has all activated abilities of all cards exiled with it</examplecapture>
[Dependent]
public class HasAllActivatedAbilities : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), "all activated abilities of", Opt("all"), Prop(Source), Prop(Duration)];

    public PermanentVerb Verb { get; set; }
    public OneOf<AllPermanents, EachObject, CardsInZoneGroup, CardsExiledWithThis, PermanentPhrase> Source { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
