namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Abandoned Sarcophagus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Abandoned Sarcophagus</exampledoc>
[TokenizationOrder(-1)]
public class AbandonedSarcophagusCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast spells that have a cycling ability from your graveyard"];
}
