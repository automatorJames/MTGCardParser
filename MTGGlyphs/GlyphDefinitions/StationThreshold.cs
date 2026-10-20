namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A station card's threshold sentence: the charge-counter count at which it becomes a creature.</summary>
/// <exampledoc>Sledge-Class Seedship</exampledoc>
/// <examplecapture>it's an artifact creature at 7+</examplecapture>
public class StationThreshold : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["it's an artifact creature at ", Prop(Threshold), "+"];

    public int Threshold { get; set; }
}
