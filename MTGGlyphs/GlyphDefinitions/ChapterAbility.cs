namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A Saga's chapter ability: one or more chapter numbers, a dash, then the effect: "i — scry 2", "iii, iv — add {r}".</summary>
/// <exampledoc>The Binding of the Titans</exampledoc>
/// <examplecapture>i — each player mills three cards</examplecapture>
public class ChapterAbility : Glyph
{
    public override Nib[] Nibs => [Prop(Chapters), "—", Prop(Effect)];

    public CompoundOf<ChapterNumber> Chapters { get; set; }
    public DynamicGlyph Effect { get; set; }
}
