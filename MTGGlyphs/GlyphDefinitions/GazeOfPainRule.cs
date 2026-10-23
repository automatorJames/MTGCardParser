namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gaze of Pain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gaze of Pain</exampledoc>
/// <examplecapture>until end of turn, whenever a creature you control attacks and isn't blocked, you may choose to have it deal damage equal to its power to a target creature</examplecapture>
[TokenizationOrder(-1)]
public class GazeOfPainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, whenever a creature you control attacks and isn't blocked, you may choose to have it deal damage equal to its power to a target creature"];
}
