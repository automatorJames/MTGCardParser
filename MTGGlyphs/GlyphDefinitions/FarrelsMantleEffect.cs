namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Farrel's Mantle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Farrel's Mantle</exampledoc>
[Dependent]
public class FarrelsMantleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its controller may have it deal damage equal to its power plus 2 to another target creature"];
}
