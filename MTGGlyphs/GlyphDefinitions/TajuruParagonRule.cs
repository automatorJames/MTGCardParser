namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tajuru Paragon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tajuru Paragon</exampledoc>
[TokenizationOrder(-1)]
public class TajuruParagonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put a card that shares a creature type with it from among them into your hand"];
}
