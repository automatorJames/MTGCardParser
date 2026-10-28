namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Skeleton Scavengers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skeleton Scavengers</exampledoc>
[TokenizationOrder(-1)]
public class SkeletonScavengersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["pay {1} for each +1/+1 counter on this creature: regenerate this creature"];
}
