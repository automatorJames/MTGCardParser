namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Triad of Fates, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Triad of Fates</exampledoc>
[TokenizationOrder(-1)]
public class TriadOfFatesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{b}, {t}: exile target creature that has a fate counter on it"];
}
