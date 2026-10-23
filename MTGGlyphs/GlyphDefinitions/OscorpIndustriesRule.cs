namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Oscorp Industries, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oscorp Industries</exampledoc>
[TokenizationOrder(-1)]
public class OscorpIndustriesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mayhem (you may play this card from your graveyard if you discarded it this turn"];
}
