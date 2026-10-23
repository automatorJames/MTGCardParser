namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Boseiju, Who Shelters All, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Boseiju, Who Shelters All</exampledoc>
[TokenizationOrder(-1)]
public class BoseijuWhoSheltersAllRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that mana is spent on an instant or sorcery spell, that spell can't be countered"];
}
