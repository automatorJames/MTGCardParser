namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Krovikan Horror, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Krovikan Horror</exampledoc>
/// <examplecapture>if this card is in your graveyard with a creature card directly above it, you may return this card to your hand</examplecapture>
[Dependent]
public class KrovikanHorrorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this card is in your graveyard with a creature card directly above it, you may return this card to your hand"];
}
