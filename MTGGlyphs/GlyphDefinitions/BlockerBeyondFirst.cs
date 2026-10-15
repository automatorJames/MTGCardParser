namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"creature blocking it beyond the first": each blocker after the first, counted by rampage.</summary>
/// <exampledoc>Chromium</exampledoc>
/// <examplecapture>creature blocking it beyond the first</examplecapture>
[Dependent]
public class BlockerBeyondFirst : Glyph
{
    public override Nib[] Nibs => ["creature blocking", Prop(Blocked), "beyond the first"];

    public It Blocked { get; set; }
}
