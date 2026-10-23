namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lobotomy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lobotomy</exampledoc>
[TokenizationOrder(-1)]
public class LobotomyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player reveals their hand, then you choose a card other than a basic land card from it"];
}
