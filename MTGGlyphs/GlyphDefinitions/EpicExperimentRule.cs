namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Epic Experiment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Epic Experiment</exampledoc>
[TokenizationOrder(-1)]
public class EpicExperimentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then put all cards exiled this way that weren't cast into your graveyard"];
}
