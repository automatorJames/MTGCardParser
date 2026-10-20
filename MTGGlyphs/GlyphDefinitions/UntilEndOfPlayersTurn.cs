namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"until the end of your next turn", "until the end of your turn": a duration ending with a player's turn.</summary>
/// <exampledoc>Alania's Pathmaker</exampledoc>
/// <examplecapture>until the end of your next turn</examplecapture>
[Dependent]
public class UntilEndOfPlayersTurn : Glyph
{
    public override Nib[] Nibs => ["until the end of", Prop(Turn)];

    public PlayersPhase Turn { get; set; }
}
