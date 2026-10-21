namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Reminder rule explaining a tap or untap symbol: "{q} is the untap symbol". Held by ReminderText.</summary>
/// <exampledoc>Pili-Pala</exampledoc>
/// <examplecapture>{q} is the untap symbol</examplecapture>
[Dependent]
public class SymbolMeaning : Glyph
{
    public override Nib[] Nibs => [Prop(Symbol), "is the", Alt("untap", "tap"), "symbol"];

    public TapSymbol Symbol { get; set; }
}
