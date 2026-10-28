namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Confusion in the Ranks, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Confusion in the Ranks</exampledoc>
[TokenizationOrder(-1)]
public class ConfusionInTheRanksRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange control of those permanents"];
}
