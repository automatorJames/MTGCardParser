namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hallow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hallow</exampledoc>
[TokenizationOrder(-1)]
public class HallowRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage target spell would deal this turn"];
}
