namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Abandoned Sarcophagus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Abandoned Sarcophagus</exampledoc>
[TokenizationOrder(-1)]
public class AbandonedSarcophagusRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a card that has a cycling ability would be put into your graveyard from anywhere and it wasn't cycled, exile it instead"];
}
