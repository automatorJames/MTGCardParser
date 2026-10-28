namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chaos Orb, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chaos Orb</exampledoc>
[TokenizationOrder(-1)]
public class ChaosOrbRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this artifact turns over completely at least once during the flip, destroy all nontoken permanents it touches"];
}
