namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Overzealous Muscle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Overzealous Muscle</exampledoc>
[TokenizationOrder(-1)]
public class OverzealousMuscleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage and effects that say \"destroy\" don't destroy a creature with indestructible"];
}
