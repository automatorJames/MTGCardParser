namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mardu Siegebreaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mardu Siegebreaker</exampledoc>
[Dependent]
public class MarduSiegebreakerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each opponent, create a tapped token that's a copy of the exiled card attacking that opponent"];
}
