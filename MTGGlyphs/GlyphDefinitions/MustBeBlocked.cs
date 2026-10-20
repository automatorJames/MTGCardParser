namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A lure requirement: "must be blocked if able", "must be blocked this turn if able". Also the second of two predicates.</summary>
/// <exampledoc>Goblin Fire Fiend</exampledoc>
/// <examplecapture>must be blocked if able</examplecapture>
[Dependent]
public class MustBeBlocked : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["must be blocked", Prop(Period), "if able"];

    [Optional]
    public ThisPeriod Period { get; set; }
}
