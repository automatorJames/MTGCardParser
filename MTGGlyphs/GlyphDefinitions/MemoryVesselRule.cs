namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Memory Vessel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Memory Vessel</exampledoc>
[TokenizationOrder(-1)]
public class MemoryVesselRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next turn, players may play cards they exiled this way, and they can't play cards from their hand"];
}
