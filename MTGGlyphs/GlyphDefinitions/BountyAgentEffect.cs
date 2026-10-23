namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bounty Agent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bounty Agent</exampledoc>
[Dependent]
public class BountyAgentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target legendary permanent that's an artifact, creature, or enchantment"];
}
