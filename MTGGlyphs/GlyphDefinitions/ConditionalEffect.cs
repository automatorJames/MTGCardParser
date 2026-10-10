namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A conditional effect: "if you control an urza's mine and an urza's tower, add {c}{c} instead". The condition isn't modelled yet; the effect resolves when it can.</summary>
/// <exampledoc>Urza's Tower</exampledoc>
/// <examplecapture>if you control an urza's mine and an urza's power-plant, add {c}{c}{c} instead</examplecapture>
public class ConditionalEffect : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Condition), ",", Prop(Effect)];

    [AllowUnmatched]
    public DynamicGlyph Condition { get; set; }
    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }
}
