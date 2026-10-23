namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Experiment Kraj, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Experiment Kraj</exampledoc>
[TokenizationOrder(-1)]
public class ExperimentKrajRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "has all activated abilities of each other creature with a +1/+1 counter on it"];
}
