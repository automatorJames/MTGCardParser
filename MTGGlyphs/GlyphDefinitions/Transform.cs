namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Turning a double-faced permanent over: "transform this creature", "transform it". Also a predicate, so it can follow a cost or a condition, or be the second of two actions.</summary>
/// <exampledoc>Ulvenwald Captive // Ulvenwald Abomination</exampledoc>
/// <examplecapture>transform this creature</examplecapture>
public class Transform : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["transform", Prop(Permanent)];

    public PermanentPhrase Permanent { get; set; }
}
