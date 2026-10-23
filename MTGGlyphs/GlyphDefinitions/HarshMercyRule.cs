namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Harsh Mercy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Harsh Mercy</exampledoc>
[TokenizationOrder(-1)]
public class HarshMercyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures that aren't of a type chosen this way"];
}
