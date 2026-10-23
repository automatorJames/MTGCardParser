namespace MTGGlyphs.GlyphDefinitions;

/// <summary>You and every permanent of a kind, as one damage recipient: "you and each creature you control". ("each opponent and each creature they control" is EachObjects.)</summary>
/// <exampledoc>Sorrow's Path</exampledoc>
/// <examplecapture>you and each creature you control</examplecapture>
[Dependent]
public class YouAndEach : Glyph
{
    public override Nib[] Nibs => ["you and", Prop(Each)];

    public OneOf<EachPermanents, EachObject> Each { get; set; }
}
