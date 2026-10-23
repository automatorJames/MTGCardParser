namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Immortal Sun, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Immortal Sun</exampledoc>
[TokenizationOrder(-1)]
public class TheImmortalSunRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players can't activate planeswalkers' loyalty abilities"];
}
