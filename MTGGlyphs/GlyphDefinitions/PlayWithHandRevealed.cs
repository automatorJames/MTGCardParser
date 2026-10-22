namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static ability keeping players' hands revealed: "players play with their hands revealed", "your opponents play with their hands revealed", "play with your hand revealed".</summary>
/// <exampledoc>Revelation</exampledoc>
/// <examplecapture>players play with their hands revealed</examplecapture>
public class PlayWithHandRevealed : Glyph
{
    public override Nib[] Nibs => [Prop(Players), "play with", Prop(Whose), Pattern("hands?"), "revealed"];

    [Optional]
    public WhichPlayer? Players { get; set; }
    public Whose Whose { get; set; }
}
