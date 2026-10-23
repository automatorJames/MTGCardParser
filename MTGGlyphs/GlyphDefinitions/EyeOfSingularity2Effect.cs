namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Eye of Singularity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eye of Singularity</exampledoc>
[Dependent]
public class EyeOfSingularity2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all other permanents with that name"];
}
