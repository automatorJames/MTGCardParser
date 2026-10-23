namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Elemental Teachings, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elemental Teachings</exampledoc>
[TokenizationOrder(-1)]
public class ElementalTeachingsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put the chosen cards into your graveyard and the rest onto the battlefield tapped, then shuffle"];
}
