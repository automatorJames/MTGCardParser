namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kaya, Spirits' Justice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaya, Spirits' Justice</exampledoc>
[TokenizationOrder(-1)]
public class KayaSpiritsJusticeCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever one or more creatures you control and/or creature cards in your graveyard are put into exile, you may choose a creature card from among them"];
}
