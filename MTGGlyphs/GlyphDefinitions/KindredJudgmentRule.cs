namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kindred Judgment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kindred Judgment</exampledoc>
[TokenizationOrder(-1)]
public class KindredJudgmentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures that aren't of the chosen type"];
}
