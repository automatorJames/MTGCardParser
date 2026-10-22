namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"its controller's next untap step", "your next untap step", "their controller's next untap step", "their controllers' next untap steps" (each of several players'): the next occurrence of a player's step.</summary>
/// <exampledoc>Crippling Chill</exampledoc>
/// <examplecapture>its controller's next untap step</examplecapture>
[Dependent]
public class NextPlayersPhase : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), "next", Plural(Prop(Phase))];

    public Whose Whose { get; set; }
    public Phase Phase { get; set; }
}
