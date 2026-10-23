namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Magmatic Galleon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Magmatic Galleon</exampledoc>
[TokenizationOrder(-1)]
public class MagmaticGalleonCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever one or more creatures your opponents control are dealt excess noncombat damage, create a treasure token"];
}
