namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Moonlight Bargain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moonlight Bargain</exampledoc>
[TokenizationOrder(-1)]
public class MoonlightBargainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each card, put that card into your graveyard unless you pay 2 life"];
}
