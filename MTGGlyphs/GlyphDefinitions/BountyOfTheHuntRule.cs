namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bounty of the Hunt, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bounty of the Hunt</exampledoc>
/// <examplecapture>for each +1/+1 counter you put on a creature this way, remove a +1/+1 counter from that creature at the beginning of the next cleanup step</examplecapture>
[TokenizationOrder(-1)]
public class BountyOfTheHuntRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each +1/+1 counter you put on a creature this way, remove a +1/+1 counter from that creature at the beginning of the next cleanup step"];
}
