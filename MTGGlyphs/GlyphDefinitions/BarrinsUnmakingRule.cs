namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Barrin's Unmaking, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Barrin's Unmaking</exampledoc>
/// <examplecapture>return target permanent to its owner's hand if that permanent shares a color with the most common color among all permanents or a color tied for most common</examplecapture>
[TokenizationOrder(-1)]
public class BarrinsUnmakingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target permanent to its owner's hand if that permanent shares a color with the most common color among all permanents or a color tied for most common"];
}
