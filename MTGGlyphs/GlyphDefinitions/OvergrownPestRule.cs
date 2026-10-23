namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Overgrown Pest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Overgrown Pest</exampledoc>
[TokenizationOrder(-1)]
public class OvergrownPestRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may reveal a land or double-faced card from among them and put that card into your hand"];
}
