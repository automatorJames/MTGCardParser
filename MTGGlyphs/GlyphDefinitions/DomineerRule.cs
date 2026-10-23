namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Domineer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Domineer</exampledoc>
[TokenizationOrder(-1)]
public class DomineerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you control enchanted artifact creature"];
}
