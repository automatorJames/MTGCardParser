namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Camouflage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Camouflage</exampledoc>
[TokenizationOrder(-1)]
public class CamouflageRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures those players control that can block additional creatures may likewise be put into additional piles"];
}
