namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mana Clash, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mana Clash</exampledoc>
[TokenizationOrder(-1)]
public class ManaClashRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["repeat this process until both players' coins come up heads on the same flip"];
}
