namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"can only attack alone": a creature that may attack only when no other creature attacks. Also the second of two predicates: "gets +3/+0 and can only attack alone".</summary>
/// <exampledoc>Master of Cruelties</exampledoc>
/// <examplecapture>can only attack alone</examplecapture>
[Dependent]
public class CanOnlyActAlone : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can only", Prop(Action), "alone"];

    public CombatAction Action { get; set; }
}
