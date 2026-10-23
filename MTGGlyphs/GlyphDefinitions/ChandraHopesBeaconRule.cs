namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chandra, Hope's Beacon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra, Hope's Beacon</exampledoc>
[TokenizationOrder(-1)]
public class ChandraHopesBeaconRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until the end of your next turn, you may cast an instant or sorcery spell from among those exiled cards"];
}
