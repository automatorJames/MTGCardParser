namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Perennation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Perennation</exampledoc>
[TokenizationOrder(-1)]
public class PerennationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target permanent card from your graveyard to the battlefield with a hexproof counter and an indestructible counter on it"];
}
