namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Darkpact, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Darkpact</exampledoc>
[TokenizationOrder(-1)]
public class DarkpactRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange that card with the top card of your library"];
}
