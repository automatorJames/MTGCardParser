namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Eye of Singularity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eye of Singularity</exampledoc>
[TokenizationOrder(-1)]
public class EyeOfSingularityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a permanent other than a basic land enters, destroy all other permanents with that name"];
}
