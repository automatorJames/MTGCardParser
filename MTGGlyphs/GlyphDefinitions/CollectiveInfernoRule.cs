namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Collective Inferno, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Collective Inferno</exampledoc>
[TokenizationOrder(-1)]
public class CollectiveInfernoRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["double all damage that sources you control of the chosen type would deal"];
}
