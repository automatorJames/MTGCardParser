namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Balthor the Defiled, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balthor the Defiled</exampledoc>
[Dependent]
public class BalthorTheDefiledEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player returns all black and all red creature cards from their graveyard to the battlefield"];
}
