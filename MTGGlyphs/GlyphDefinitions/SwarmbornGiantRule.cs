namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Swarmborn Giant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swarmborn Giant</exampledoc>
[TokenizationOrder(-1)]
public class SwarmbornGiantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you're dealt combat damage, sacrifice this creature"];
}
