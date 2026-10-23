namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Purity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Purity</exampledoc>
[TokenizationOrder(-1)]
public class PurityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if noncombat damage would be dealt to you, prevent that damage"];
}
