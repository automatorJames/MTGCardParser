namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Goblin Gathering, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Gathering</exampledoc>
[TokenizationOrder(-1)]
public class GoblinGatheringRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a number of 1/1 red goblin creature tokens equal to two plus the number of cards named", Nib.This, "in your graveyard"];
}
