namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Unified Will, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unified Will</exampledoc>
[TokenizationOrder(-1)]
public class UnifiedWillRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell if you control more creatures than that spell's controller"];
}
