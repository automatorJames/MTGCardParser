namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Reminder text: a parenthesized restatement of a rule, "(this creature can't attack" - after a keyword, or on its own after an ability that grants one. Its content is held as an effect until something models it.</summary>
/// <exampledoc>Living Wall</exampledoc>
/// <examplecapture>(this creature can't attack</examplecapture>
public class ReminderText : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["(", Prop(Rule)];

    [AllowUnmatched]
    public DynamicGlyph Rule { get; set; }
}
