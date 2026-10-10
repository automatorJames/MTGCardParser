namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object's possessive: "that creature's", "{this}'s", "the sacrificed artifact's".</summary>
/// <exampledoc>Diamond Valley</exampledoc>
/// <examplecapture>the sacrificed creature's</examplecapture>
[Dependent]
public class ObjectPossessive : Glyph
{
    public override Nib[] Nibs => [Prop(Object), "'s"];

    public OneOf<This, ThatCard, SacrificedPermanent> Object { get; set; }
}
