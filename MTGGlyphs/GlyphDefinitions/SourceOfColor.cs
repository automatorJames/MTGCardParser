namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"from a red source": a damage source by color.</summary>
/// <exampledoc>Honorable Passage</exampledoc>
/// <examplecapture>from a red source</examplecapture>
[Dependent]
public class SourceOfColor : Glyph
{
    public override Nib[] Nibs => ["from a", Prop(Color), "source"];

    public ManaColor Color { get; set; }
}
