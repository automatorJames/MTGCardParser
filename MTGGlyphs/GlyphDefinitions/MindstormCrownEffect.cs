namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mindstorm Crown, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mindstorm Crown</exampledoc>
[Dependent]
public class MindstormCrownEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card if you had no cards in hand at the beginning of this turn"];
}
