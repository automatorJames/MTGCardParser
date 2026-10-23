namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Magma Pummeler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Magma Pummeler</exampledoc>
[TokenizationOrder(-1)]
public class MagmaPummelerCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when one or more counters are removed from this creature this way, it deals that much damage to any target"];
}
