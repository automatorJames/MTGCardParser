namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Uba Mask, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Uba Mask</exampledoc>
[TokenizationOrder(-1)]
public class UbaMaskRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player may play lands and cast spells from among cards they exiled with this artifact this turn"];
}
