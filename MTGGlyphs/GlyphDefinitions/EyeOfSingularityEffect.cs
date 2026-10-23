namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Eye of Singularity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eye of Singularity</exampledoc>
[Dependent]
public class EyeOfSingularityEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each permanent with the same name as another permanent, except for basic lands"];
}
