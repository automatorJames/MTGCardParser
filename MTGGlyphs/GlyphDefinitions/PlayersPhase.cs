namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A step or phase, usually named by whose it is first: "your upkeep", "each player's upkeep", "the chosen player's upkeep", "their controllers' untap steps"; or no one's in particular, "combat".</summary>
/// <exampledoc>Juzám Djinn</exampledoc>
/// <examplecapture>your upkeep</examplecapture>
[Dependent]
public class PlayersPhase : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), Plural(Prop(Phase))];

    [Optional]
    public OneOf<EachPlayers, ChosenPlayers, Whose?> Whose { get; set; }
    public Phase Phase { get; set; }
}
