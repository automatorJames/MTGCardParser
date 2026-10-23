namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Butcher Orgg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Butcher Orgg</exampledoc>
[TokenizationOrder(-1)]
public class ButcherOrggRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may assign this creature's combat damage divided as you choose among defending player and/or any number of creatures they control"];
}
