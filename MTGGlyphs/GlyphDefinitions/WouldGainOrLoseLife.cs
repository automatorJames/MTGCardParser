namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player about to gain or lose life, as a replacement condition: "you would gain life", "an opponent would lose life".</summary>
/// <exampledoc>Angel of Vitality</exampledoc>
/// <examplecapture>you would gain life</examplecapture>
[Dependent]
public class WouldGainOrLoseLife : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "would", Prop(GainOrLose), "life"];

    public Subject Player { get; set; }
    public GainOrLose GainOrLose { get; set; }
}
