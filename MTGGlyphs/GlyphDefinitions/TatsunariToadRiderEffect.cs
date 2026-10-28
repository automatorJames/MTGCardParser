namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tatsunari, Toad Rider, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tatsunari, Toad Rider</exampledoc>
[Dependent]
public class TatsunariToadRiderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tatsunari and target frog you control can't be blocked this turn except by creatures with flying or reach"];
}
