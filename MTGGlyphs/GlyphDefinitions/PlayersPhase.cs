namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A step or phase named by whose it is first: "your upkeep", "each player's upkeep", "their controllers' untap steps".</summary>
/// <exampledoc>Juzám Djinn</exampledoc>
/// <examplecapture>your upkeep</examplecapture>
[Dependent]
public class PlayersPhase : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), Plural(Prop(Phase))];

    public OneOf<EachPlayers, Whose?> Whose { get; set; }
    public Phase Phase { get; set; }
}
