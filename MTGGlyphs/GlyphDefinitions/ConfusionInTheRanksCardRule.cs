namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Confusion in the Ranks, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Confusion in the Ranks</exampledoc>
[TokenizationOrder(-1)]
public class ConfusionInTheRanksCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an artifact, creature, or enchantment enters, its controller chooses target permanent another player controls that shares a card type with it"];
}
