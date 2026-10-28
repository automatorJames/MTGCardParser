namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Balthor the Defiled, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balthor the Defiled</exampledoc>
[TokenizationOrder(-1)]
public class BalthorTheDefiledRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{b}{b}{b}, exile balthor: each player returns all black and all red creature cards from their graveyard to the battlefield"];
}
