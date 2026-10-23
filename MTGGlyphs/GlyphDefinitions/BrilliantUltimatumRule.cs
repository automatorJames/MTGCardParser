namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Brilliant Ultimatum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brilliant Ultimatum</exampledoc>
[TokenizationOrder(-1)]
public class BrilliantUltimatumRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may play lands and cast spells from one of those piles"];
}
