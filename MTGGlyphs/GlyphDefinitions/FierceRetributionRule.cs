namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fierce Retribution, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fierce Retribution</exampledoc>
[TokenizationOrder(-1)]
public class FierceRetributionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target [attacking] creature"];
}
