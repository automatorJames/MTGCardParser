namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Countering a spell or ability: "counter target spell", "counter target red spell", "counter target activated ability", one named earlier ("counter that spell", "counter that spell or ability", "counter it" for ward), or the card itself, "counter {this}", "counter this spell". Also a predicate, so "you may counter target spell" resolves, and the second of two actions.</summary>
/// <exampledoc>Counterspell</exampledoc>
/// <examplecapture>counter target spell</examplecapture>
public class CounterSpell : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["counter", Prop(Countered)];

    public OneOf<TargetAbility, TargetSpell, It, This, ThisSpell, ThatStackObject> Countered { get; set; }
}
