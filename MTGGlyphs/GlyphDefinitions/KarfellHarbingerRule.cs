namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Karfell Harbinger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karfell Harbinger</exampledoc>
[TokenizationOrder(-1)]
public class KarfellHarbingerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only to foretell a card from your hand or cast an instant or sorcery spell"];
}
