namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sea Gate Restoration // Sea Gate, Reborn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sea Gate Restoration // Sea Gate, Reborn</exampledoc>
[TokenizationOrder(-1)]
public class SeaGateRestorationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw cards equal to the number of cards in your hand plus one"];
}
