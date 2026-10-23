namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bronze Tablet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bronze Tablet</exampledoc>
[TokenizationOrder(-1)]
public class BronzeTabletRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, that player owns this card and you own the other exiled card"];
}
