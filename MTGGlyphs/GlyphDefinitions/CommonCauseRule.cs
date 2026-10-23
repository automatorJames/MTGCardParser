namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Common Cause, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Common Cause</exampledoc>
[TokenizationOrder(-1)]
public class CommonCauseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["nonartifact creatures get +2/+2 as long as they all share a color"];
}
