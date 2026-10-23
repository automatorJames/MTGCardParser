namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Geistlight Snare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Geistlight Snare</exampledoc>
[TokenizationOrder(-1)]
public class GeistlightSnareRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it also costs {1} less to cast if you control an enchantment"];
}
