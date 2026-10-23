namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Predator's Rapport, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Predator's Rapport</exampledoc>
[TokenizationOrder(-1)]
public class PredatorsRapportRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain life equal to that creature's power plus its toughness"];
}
