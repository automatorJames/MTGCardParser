namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Devotion against a threshold, as a condition: "your devotion to white and black is less than seven" (the Theros gods, which aren't creatures until it's met).</summary>
/// <exampledoc>Athreos, God of Passage</exampledoc>
/// <examplecapture>your devotion to white and black is less than seven</examplecapture>
[Dependent]
public class DevotionThreshold : Glyph
{
    public override Nib[] Nibs => [Prop(Devotion), "is less than", Prop(Threshold)];

    public DevotionTo Devotion { get; set; }
    public Quantity Threshold { get; set; }
}
