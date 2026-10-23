namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Blitzball, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blitzball</exampledoc>
[TokenizationOrder(-1)]
public class BlitzballRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only if an opponent was dealt combat damage by a legendary creature this turn"];
}
