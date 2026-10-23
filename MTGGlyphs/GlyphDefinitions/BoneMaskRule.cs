namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bone Mask, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bone Mask</exampledoc>
[TokenizationOrder(-1)]
public class BoneMaskRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile cards from the top of your library equal to the damage prevented this way"];
}
