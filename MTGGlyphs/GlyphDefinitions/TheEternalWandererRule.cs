namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Eternal Wanderer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Eternal Wanderer</exampledoc>
[TokenizationOrder(-1)]
public class TheEternalWandererRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player sacrifices all creatures they control not chosen this way"];
}
