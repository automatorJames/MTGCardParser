namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One mode of a modal spell or ability: "• counter target red spell".</summary>
/// <exampledoc>Blue Elemental Blast</exampledoc>
/// <examplecapture>• counter target red spell</examplecapture>
public class Mode : Glyph
{
    public override Nib[] Nibs => ["•", Prop(Effect)];

    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }
}
