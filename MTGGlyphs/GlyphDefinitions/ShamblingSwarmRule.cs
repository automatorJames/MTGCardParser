namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shambling Swarm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shambling Swarm</exampledoc>
/// <examplecapture>for each -1/-1 counter you put on a creature this way, remove a -1/-1 counter from that creature at the beginning of the next end step</examplecapture>
[TokenizationOrder(-1)]
public class ShamblingSwarmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each -1/-1 counter you put on a creature this way, remove a -1/-1 counter from that creature at the beginning of the next end step"];
}
