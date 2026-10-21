namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Read ahead's reminder: "choose a chapter and start with that many lore counters". Held by ReminderText.</summary>
/// <exampledoc>The World Spell</exampledoc>
/// <examplecapture>choose a chapter and start with that many lore counters</examplecapture>
[Dependent]
public class ReadAheadRule : Glyph
{
    public override Nib[] Nibs => ["choose a chapter and start with that many lore counters"];
}
