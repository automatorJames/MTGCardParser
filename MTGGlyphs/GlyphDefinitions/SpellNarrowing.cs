namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What narrows a spell after its noun: what it targets ("that targets this creature"), its chosen color or type ("of the chosen color"), where it's cast from ("from your graveyard"), or when ("during an opponent's turn").</summary>
/// <exampledoc>Heroes of the Revel</exampledoc>
/// <examplecapture>that targets this creature</examplecapture>
[Dependent]
public class SpellNarrowing : GlyphOneOf
{
    public SpellTargets Targets { get; set; }
    public ChosenAspect? Chosen { get; set; }
    public CastFromZone From { get; set; }
    public DuringTurn During { get; set; }
}
