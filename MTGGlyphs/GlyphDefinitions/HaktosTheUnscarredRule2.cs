namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Haktos the Unscarred, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haktos the Unscarred</exampledoc>
[TokenizationOrder(-1)]
public class HaktosTheUnscarredRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["haktos has protection from each mana value other than the chosen number"];
}
