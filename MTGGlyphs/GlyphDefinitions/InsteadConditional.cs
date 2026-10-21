namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A conditional effect that replaces the usual one, with "instead" leading the effect: "if this spell was kicked, instead search your library for up to two basic land cards, …", "if you would draw a card, instead reveal the top three cards of your library". The sibling of <see cref="ConditionalInstead"/>, where "instead" ends the line.</summary>
/// <exampledoc>Sages of the Anima</exampledoc>
/// <examplecapture>if you would draw a card, instead reveal the top three cards of your library</examplecapture>
public class InsteadConditional : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Condition), ", instead", Prop(Effect)];

    public DynamicGlyph Condition { get; set; }
    public DynamicGlyph Effect { get; set; }
}
