namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Snow Devil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Snow Devil</exampledoc>
[TokenizationOrder(-1)]
public class SnowDevilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature has first strike as long as it's blocking and you control a snow land"];
}
