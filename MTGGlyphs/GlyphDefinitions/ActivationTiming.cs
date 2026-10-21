namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One condition on when an ability may be activated or a spell cast: during a step or turn, before a point in it, once each turn, only once, as a sorcery, or only if the game is in some state (IfState's narrower set first, then any GameState).</summary>
/// <exampledoc>Instill Energy</exampledoc>
/// <examplecapture>during your turn</examplecapture>
[Dependent]
public class ActivationTiming : GlyphOneOf
{
    public DuringPhase During { get; set; }
    public BeforePoint Before { get; set; }
    [RegexPattern("(only )?once each turn")]
    public bool OnceEachTurn { get; set; }
    [RegexPattern("(only )?once")]
    public bool Once { get; set; }
    [RegexPattern("as a sorcery")]
    public bool AsASorcery { get; set; }
    public IfState OnlyIf { get; set; }
    public IfGameState OnlyIfState { get; set; }
}
