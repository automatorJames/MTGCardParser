namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Starnheim Unleashed, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Starnheim Unleashed</exampledoc>
[TokenizationOrder(-1)]
public class StarnheimUnleashedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was foretold, create x of those tokens instead"];
}
