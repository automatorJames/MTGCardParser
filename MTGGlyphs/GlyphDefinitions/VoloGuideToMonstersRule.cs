namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Volo, Guide to Monsters, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Volo, Guide to Monsters</exampledoc>
[TokenizationOrder(-1)]
public class VoloGuideToMonstersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast a creature spell that doesn't share a creature type with a creature you control or a creature card in your graveyard, copy that spell"];
}
