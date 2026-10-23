namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Camel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Camel</exampledoc>
[TokenizationOrder(-1)]
public class CamelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as this creature is attacking, prevent all damage deserts would deal to this creature and to creatures banded with this creature"];
}
