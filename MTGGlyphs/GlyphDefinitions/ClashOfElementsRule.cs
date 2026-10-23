namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Clash of Elements, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Clash of Elements</exampledoc>
[TokenizationOrder(-1)]
public class ClashOfElementsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they didn't put the card on top of their library, they put it on the bottom"];
}
