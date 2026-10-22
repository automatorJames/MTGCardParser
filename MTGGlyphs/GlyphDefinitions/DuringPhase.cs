namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"during your upkeep", "during your turn", "during combat before blockers are declared", "during combat on your turn", "during combat after blockers are declared": throughout one player's step, phase or turn, or part of it.</summary>
/// <exampledoc>Colossus of Sardia</exampledoc>
/// <examplecapture>during your upkeep</examplecapture>
[Dependent]
public class DuringPhase : Glyph
{
    public override Nib[] Nibs => ["during", Prop(Step), Prop(OnYourTurn), Prop(Before)];

    public PlayersPhase Step { get; set; }
    [Optional]
    [RegexPattern("on your turn")]
    public bool? OnYourTurn { get; set; }
    [Optional]
    public BeforePoint Before { get; set; }
}
