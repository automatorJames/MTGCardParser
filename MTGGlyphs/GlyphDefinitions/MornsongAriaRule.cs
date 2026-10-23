namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mornsong Aria, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mornsong Aria</exampledoc>
[TokenizationOrder(-1)]
public class MornsongAriaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players can't draw cards or gain life"];
}
