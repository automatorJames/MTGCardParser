namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Exile, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Exile</exampledoc>
[TokenizationOrder(-1)]
public class ExileRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "target nonwhite attacking creature"];
}
