namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player picked out as the extreme of something: "the player with the most life", "the player with the lowest life total", "the player who has the most cards in hand", "the player who controls the most creatures".</summary>
/// <exampledoc>Wild Mammoth</exampledoc>
/// <examplecapture>the player who controls the most creatures</examplecapture>
[Dependent]
public class PlayerWithMost : Glyph
{
    public override Nib[] Nibs => ["the player", Prop(Measure)];

    public OneOf<PlayerSuperlative?, ControlsTheMost> Measure { get; set; }
}
