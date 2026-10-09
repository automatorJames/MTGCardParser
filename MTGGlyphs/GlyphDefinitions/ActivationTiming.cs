namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One condition on when an ability may be activated: during a step or turn, once each turn, or as a sorcery.</summary>
/// <exampledoc>Instill Energy</exampledoc>
/// <examplecapture>during your turn</examplecapture>
[Dependent]
public class ActivationTiming : GlyphOneOf
{
    public DuringPhase During { get; set; }
    [RegexPattern("(only )?once each turn")]
    public bool OnceEachTurn { get; set; }
    [RegexPattern("as a sorcery")]
    public bool AsASorcery { get; set; }
}
