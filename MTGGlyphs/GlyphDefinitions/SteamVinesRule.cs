namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Steam Vines, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Steam Vines</exampledoc>
[TokenizationOrder(-1)]
public class SteamVinesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player attaches this aura to a land of their choice"];
}
