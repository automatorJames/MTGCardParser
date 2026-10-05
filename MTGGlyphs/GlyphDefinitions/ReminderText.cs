namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// Parenthesized reminder text, which restates a rule and has no rules meaning of its own: "flying (this creature
/// can't be blocked except by creatures with flying or reach.)".
/// </summary>
public class ReminderText : Glyph
{
    public override Nib[] Nibs => ["(", Prop(Reminder), Opt("."), ")"];

    [AllowUnmatched]
    public DynamicGlyph Reminder { get; set; }
}
