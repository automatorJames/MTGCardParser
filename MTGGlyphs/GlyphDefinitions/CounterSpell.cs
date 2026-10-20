namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Countering a spell: "counter target spell", "counter target red spell", one named earlier, "counter it" (ward), or the card itself, "counter {this}", "counter this spell". Also a predicate, so "you may counter target spell" resolves, and the second of two actions.</summary>
/// <exampledoc>Counterspell</exampledoc>
/// <examplecapture>counter target spell</examplecapture>
public class CounterSpell : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["counter", Prop(Countered)];

    public OneOf<TargetSpell, It, OneOf<This, ThisSpell>> Countered { get; set; }
}
