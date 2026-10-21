namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static ability revealing the top card of a library for as long as it applies: "play with the top card of your library revealed", "players play with the top card of their libraries revealed".</summary>
/// <exampledoc>Courser of Kruphix</exampledoc>
/// <examplecapture>play with the top card of your library revealed</examplecapture>
public class PlayWithTopCardRevealed : Glyph
{
    public override Nib[] Nibs => [Prop(AllPlayers), "play with the top card of", Prop(Whose), Pattern("librar(y|ies)"), "revealed"];

    [RegexPattern("players")]
    public bool AllPlayers { get; set; }
    public Whose Whose { get; set; }
}
