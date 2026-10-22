namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Impending's reminder effect: "it enters with four time counters and isn't a creature until the last is removed" - the counters it enters with, and that it isn't a creature while any remain.</summary>
/// <exampledoc>Overlord of the Hauntwoods</exampledoc>
/// <examplecapture>it enters with four time counters and isn't a creature until the last is removed</examplecapture>
public class EntersNotCreatureUntilCountersRemoved : Glyph
{
    public override Nib[] Nibs => ["it enters with", Prop(Counters), "and isn't a creature until the last is removed"];

    public Counters Counters { get; set; }
}
