namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Stock reminder sentences that restate a rule, alone or in parentheses: "(zero is even", "(piles can be empty", "round up each time", "(they're affected by summoning sickness", "(you can't look at it", "(do this before you draw", "(it's every creature type", "(this effect doesn't end". Fixed rules text with nothing that varies but which rule it is.</summary>
/// <exampledoc>Void Winnower</exampledoc>
/// <examplecapture>zero is even</examplecapture>
public class StockReminder : Glyph
{
    public ReminderRule Rule { get; set; }
}
