namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ichormoon Gauntlet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ichormoon Gauntlet</exampledoc>
[Dependent]
public class IchormoonGauntletEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a counter on target permanent"];
}
