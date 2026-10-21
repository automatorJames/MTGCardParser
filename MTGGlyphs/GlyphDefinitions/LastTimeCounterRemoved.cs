namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The time-counter trigger of suspend and vanishing reminder text: "when the last is removed, you may cast it without paying its mana cost" (suspend), "when the last is removed, sacrifice it" (vanishing). "the last" is the last time counter.</summary>
/// <exampledoc>Rift Bolt</exampledoc>
/// <examplecapture>when the last is removed, you may cast it without paying its mana cost</examplecapture>
public class LastTimeCounterRemoved : Glyph
{
    public override Nib[] Nibs => ["when the last is removed,", Prop(Effect)];

    public DynamicGlyph Effect { get; set; }
}
