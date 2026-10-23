namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"instead …" leading a replacement effect: "instead put a card you own from outside the game into your hand".</summary>
/// <exampledoc>Ring of Ma'rûf</exampledoc>
/// <examplecapture>instead put a card you own from outside the game into your hand</examplecapture>
[Dependent]
public class InsteadThenEffect : Glyph
{
    public override Nib[] Nibs => ["instead", Prop(Effect)];

    public DynamicGlyph Effect { get; set; }
}
