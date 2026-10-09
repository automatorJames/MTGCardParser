namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword ability line with its reminder text in parentheses: "flying (this creature can't be blocked except by creatures with flying or reach.)". The reminder only restates the last keyword's rule; keywords without reminders may come first, set off by a semicolon ("flying; banding (…").</summary>
/// <exampledoc>Dancing Scimitar</exampledoc>
/// <examplecapture>flying (this creature can't be blocked except by creatures with flying or reach</examplecapture>
public class KeywordWithReminder : Glyph
{
    public override Nib[] Nibs => [Prop(Before), Prop(Keywords), Prop(Reminder)];

    [Optional]
    public KeywordsBeforeSemicolon Before { get; set; }
    public CardAbilityLine Keywords { get; set; }
    public ReminderText Reminder { get; set; }
}
