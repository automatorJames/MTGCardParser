namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Turning a double-faced permanent over: "transform this creature", "transform it". Also a predicate, so it can follow a cost or a condition.</summary>
/// <exampledoc>Ulvenwald Captive // Ulvenwald Abomination</exampledoc>
/// <examplecapture>transform this creature</examplecapture>
public class Transform : Glyph, IPredicate
{
    public override Nib[] Nibs => ["transform", Prop(Permanent)];

    public PermanentPhrase Permanent { get; set; }
}
