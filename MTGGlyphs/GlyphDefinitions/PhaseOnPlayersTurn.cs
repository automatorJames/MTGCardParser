namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A step or phase in a given player's turn: "combat on your turn", "combat on each player's turn".</summary>
/// <exampledoc>Siege Veteran</exampledoc>
/// <examplecapture>combat on your turn</examplecapture>
[Dependent]
public class PhaseOnPlayersTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Phase), "on", Prop(Whose), "turn"];

    public Phase Phase { get; set; }
    public OneOf<EachPlayers, Whose?> Whose { get; set; }
}
