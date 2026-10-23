namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Diligent Zookeeper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Diligent Zookeeper</exampledoc>
[TokenizationOrder(-1)]
public class DiligentZookeeperRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each non-human creature you control gets +1/+1 for each of its creature types, to a maximum of 10"];
}
