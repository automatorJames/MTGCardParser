namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Silhouette, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Silhouette</exampledoc>
[TokenizationOrder(-1)]
public class SilhouetteRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a spell or ability that targets that creature would cause a source to deal damage to that creature this turn, prevent that damage"];
}
