namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Choosing cards from cards just revealed or looked at, or from a player's zones: "choose a nonland card from it" (a revealed hand), "chooses a creature card from among them", "choose up to two cards from it", "choose a card from it with mana value 3 or less", "choose a nonland card from that player's graveyard or hand". A predicate, so "you choose …" and "an opponent chooses …" resolve - and a conjoined one, so "look at target player's hand and choose a card from it" pairs it with <see cref="LookAt"/>.</summary>
/// <exampledoc>Distress</exampledoc>
/// <examplecapture>choose a nonland card from it</examplecapture>
[Dependent]
public class ChooseCardFrom : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("chooses?"), Opt("up to"), Prop(Count), Prop(Qualifiers), Plural("card"), Prop(With), "from", Prop(Source), Prop(TrailingWith)];

    public Quantity Count { get; set; }
    [Optional]
    public OneOf<ManyOf<TargetQualifier>, CompoundOf<TargetQualifier>, QualifierWords> Qualifiers { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
    public OneOf<It, AmongThem, PlayersZones> Source { get; set; }
    [Optional]
    public WithQualifier TrailingWith { get; set; }
}
