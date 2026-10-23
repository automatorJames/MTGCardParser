namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tithe, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tithe</exampledoc>
[TokenizationOrder(-1)]
public class TitheRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if target opponent controls more lands than you, you may search your library for an additional plains card"];
}
