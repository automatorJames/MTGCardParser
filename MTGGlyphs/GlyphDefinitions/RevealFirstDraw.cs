namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The standing instruction to reveal each turn's first draw, which "whenever you reveal a … card this way" triggers then read: "reveal the first card you draw each turn", "reveal the first card you draw on each of your turns", and with "you may", "(you may) reveal the first card you draw each turn as you draw it".</summary>
/// <exampledoc>Primitive Etchings</exampledoc>
/// <examplecapture>reveal the first card you draw each turn</examplecapture>
public class RevealFirstDraw : Glyph, IPredicate
{
    public override Nib[] Nibs => ["reveal the first card you draw", Prop(Turns), Prop(AsDrawn)];

    public FirstDrawTurns Turns { get; set; }
    [RegexPattern("as you draw it")]
    public bool AsDrawn { get; set; }
}
