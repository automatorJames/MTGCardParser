namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Unable to Scream, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unable to Scream</exampledoc>
[TokenizationOrder(-1)]
public class UnableToScreamRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as enchanted creature is face down, it can't be turned face up"];
}
