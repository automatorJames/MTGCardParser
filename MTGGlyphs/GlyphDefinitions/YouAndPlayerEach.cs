namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two players doing the same thing, each on their own: "you and target opponent each", "you and that player each", "you and an opponent each".</summary>
/// <exampledoc>Secret Rendezvous</exampledoc>
/// <examplecapture>you and target opponent each</examplecapture>
[Dependent]
public class YouAndPlayerEach : Glyph
{
    public override Nib[] Nibs => ["you and", Prop(Other), "each"];

    [Referent]
    public OneOf<WhichPlayer?, PlayerReference, SpecificTarget> Other { get; set; }
}
