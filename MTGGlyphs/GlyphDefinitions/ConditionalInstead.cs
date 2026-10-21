namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A conditional effect that replaces the usual one, ending in "instead": "if this spell was kicked, that creature gets -6/-6 until end of turn instead", "if this spell was cast using teamwork, choose both instead". Beside <see cref="ConditionalEffect"/>, because a dynamic slot ends only at a literal.</summary>
/// <exampledoc>Vicious Offering</exampledoc>
/// <examplecapture>if this spell was kicked, that creature gets -5/-5 until end of turn instead</examplecapture>
public class ConditionalInstead : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Condition), ",", Prop(Effect), "instead"];

    public DynamicGlyph Condition { get; set; }
    public DynamicGlyph Effect { get; set; }
}
