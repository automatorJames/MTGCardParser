namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Indomitable Creativity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Indomitable Creativity</exampledoc>
[TokenizationOrder(-1)]
public class IndomitableCreativityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["those players put the exiled cards onto the battlefield, then shuffle"];
}
