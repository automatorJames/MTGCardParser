namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Permission to play more lands than the one per turn the rules allow: "play an additional land on each of your turns", "play two additional lands this turn", "play up to three additional lands this turn". A predicate, so "you may …" and "each player may …" resolve to it.</summary>
/// <exampledoc>Exploration</exampledoc>
/// <examplecapture>play an additional land on each of your turns</examplecapture>
[Dependent]
public class PlayAdditionalLands : Glyph, IPredicate
{
    public override Nib[] Nibs => ["play", Prop(Count), Opt("additional"), Plural("land"), Prop(Period)];

    public OneOf<UpToCount, Quantity?> Count { get; set; }
    public OneOf<ThisPeriod, OnEachTurn> Period { get; set; }
}
