namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Feint, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Feint</exampledoc>
[TokenizationOrder(-1)]
public class FeintRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all combat damage that would be dealt this turn by that creature and each creature blocking it"];
}
