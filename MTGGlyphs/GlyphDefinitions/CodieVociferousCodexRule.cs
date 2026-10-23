namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Codie, Vociferous Codex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Codie, Vociferous Codex</exampledoc>
[TokenizationOrder(-1)]
public class CodieVociferousCodexRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put each other card exiled this way on the bottom of your library in a random order"];
}
