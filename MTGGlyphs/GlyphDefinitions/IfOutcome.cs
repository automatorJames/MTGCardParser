namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What follows the outcome of a choice, a requirement or a coin flip: "if you do, …", "if the player does, …", "if you don't, …", "if you can't, …", "if you win the flip, …". Replaces the separate "if you do" and "if the player does" glyphs.</summary>
/// <exampledoc>Crystal Rod</exampledoc>
/// <examplecapture>if you do, you gain 1 life</examplecapture>
public class IfOutcome : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Player), Prop(Result), ",", Prop(Outcome)];

    public OneOf<ThatPlayer, TheyPlayer, WhichPlayer?> Player { get; set; }
    public OutcomeResult Result { get; set; }
    [AllowUnmatched]
    public DynamicGlyph Outcome { get; set; }
}
