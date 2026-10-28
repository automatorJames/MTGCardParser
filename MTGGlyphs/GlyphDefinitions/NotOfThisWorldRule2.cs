namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Not of This World, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Not of This World</exampledoc>
[TokenizationOrder(-1)]
public class NotOfThisWorldRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this spell costs {7} less to cast if it targets a spell or ability that targets a creature you control with power 7 or greater"];
}
