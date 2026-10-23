namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lagrella, the Magpie, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lagrella, the Magpie</exampledoc>
[TokenizationOrder(-1)]
public class LagrellaTheMagpieCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when an exiled card enters under your control this way, put two +1/+1 counters on it"];
}
