namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fblthp, the Lost, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fblthp, the Lost</exampledoc>
[Dependent]
public class FblthpTheLost2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle fblthp into its owner's library"];
}
