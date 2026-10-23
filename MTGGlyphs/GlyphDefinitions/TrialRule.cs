namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Trial // Error, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Trial // Error</exampledoc>
[TokenizationOrder(-1)]
public class TrialRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return all creatures blocking or blocked by target creature to their owner's hand"];
}
