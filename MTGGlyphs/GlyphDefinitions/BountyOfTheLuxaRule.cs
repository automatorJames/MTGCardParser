namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bounty of the Luxa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bounty of the Luxa</exampledoc>
[TokenizationOrder(-1)]
public class BountyOfTheLuxaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if no counters were removed this way, put a flood counter on this enchantment and draw a card"];
}
