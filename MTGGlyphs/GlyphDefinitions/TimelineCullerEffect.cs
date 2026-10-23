namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Timeline Culler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Timeline Culler</exampledoc>
[Dependent]
public class TimelineCullerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast this card from your hand or graveyard for its warp cost"];
}
