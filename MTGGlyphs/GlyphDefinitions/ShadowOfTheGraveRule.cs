namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shadow of the Grave, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shadow of the Grave</exampledoc>
[TokenizationOrder(-1)]
public class ShadowOfTheGraveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return to your hand all cards in your graveyard that you cycled or discarded this turn"];
}
