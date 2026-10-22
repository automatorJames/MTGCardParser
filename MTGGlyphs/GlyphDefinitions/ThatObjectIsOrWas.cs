namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that creature is" / "that permanent was" / "the sacrificed creature was": an object named earlier by its type or by how it was handled, then the verb, in a condition on its kind ("if that permanent was a nissa planeswalker", "if the sacrificed creature was a human").</summary>
/// <exampledoc>Nissa's Defeat</exampledoc>
/// <examplecapture>that permanent was</examplecapture>
[Dependent]
public class ThatObjectIsOrWas : Glyph
{
    public override Nib[] Nibs => [Prop(Object), Pattern("is|was")];

    public OneOf<ThatCard, SacrificedPermanent> Object { get; set; }
}
