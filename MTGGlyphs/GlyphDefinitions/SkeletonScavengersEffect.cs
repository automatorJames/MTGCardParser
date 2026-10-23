namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Skeleton Scavengers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skeleton Scavengers</exampledoc>
[Dependent]
public class SkeletonScavengersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["regenerate this creature"];
}
