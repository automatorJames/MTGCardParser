namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Danitha, Sword of Hope, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Danitha, Sword of Hope</exampledoc>
[TokenizationOrder(-1)]
public class DanithaSwordOfHopeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast an equipment spell or a spell that targets a creature you control, draw a card"];
}
