namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Confusion in the Ranks, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Confusion in the Ranks</exampledoc>
[Dependent]
public class ConfusionInTheRanksEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creature, or enchantment enters, its controller chooses target permanent another player controls that shares a card type with it"];
}
