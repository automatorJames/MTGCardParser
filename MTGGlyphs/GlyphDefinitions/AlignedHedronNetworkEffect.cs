namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Aligned Hedron Network, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aligned Hedron Network</exampledoc>
[Dependent]
public class AlignedHedronNetworkEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["those creatures return under their owners' control"];
}
