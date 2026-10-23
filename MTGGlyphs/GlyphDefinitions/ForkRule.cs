namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fork, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fork</exampledoc>
[TokenizationOrder(-1)]
public class ForkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy target instant or sorcery spell, except that the copy is red"];
}
