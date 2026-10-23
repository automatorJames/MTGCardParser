namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gathering Stone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gathering Stone</exampledoc>
[TokenizationOrder(-1)]
public class GatheringStoneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a card of the chosen type, you may reveal it and put it into your hand"];
}
