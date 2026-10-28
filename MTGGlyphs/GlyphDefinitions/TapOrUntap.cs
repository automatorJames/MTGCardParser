namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Tapping or untapping permanents: "tap target wall", "untap {this}", "untap those creatures", "tap x target creatures", "untap all creatures you control", "untap each other creature you control", "tap each creature dealt damage this way", "tap two untapped elves you control", two at once, "untap up to one target creature and up to one target land", several whole groups, "untap all forests and all green creatures", or as a cost, "tap enchanted creature"; optionally in a step, "untap all permanents you control during each other player's untap step". Also a predicate, so "you may tap target creature" resolves.</summary>
/// <exampledoc>Ali Baba</exampledoc>
/// <examplecapture>tap target wall</examplecapture>
public class TapOrUntap : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), Prop(Permanent), Prop(During)];

    public TapAction Action { get; set; }
    public OneOf<ManyOf<AllPermanents>, PermanentPhrase, PermanentPair, IndefinitePermanent, EachDamagedObject, PermanentGroup, CountedPermanents, EachPermanents> Permanent { get; set; }
    [Optional]
    public DuringStep During { get; set; }
}
