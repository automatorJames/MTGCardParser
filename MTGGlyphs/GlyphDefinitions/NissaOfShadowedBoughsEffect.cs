namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nissa of Shadowed Boughs, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nissa of Shadowed Boughs</exampledoc>
/// <examplecapture>you may put a creature card with mana value less than or equal to the number of lands you control onto the battlefield from your hand or graveyard with two +1/+1 counters on it</examplecapture>
[Dependent]
public class NissaOfShadowedBoughsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put a creature card with mana value less than or equal to the number of lands you control onto the battlefield from your hand or graveyard with two +1/+1 counters on it"];
}
