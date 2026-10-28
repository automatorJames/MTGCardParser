namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Approach of the Second Sun, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Approach of the Second Sun</exampledoc>
[TokenizationOrder(-1)]
public class ApproachOfTheSecondSunRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, put", Nib.This, "into its owner's library seventh from the top and you gain 7 life"];
}
