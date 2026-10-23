namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Phyrexian Splicer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Splicer</exampledoc>
[TokenizationOrder(-1)]
public class PhyrexianSplicerCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{2}, {t}, choose flying, first strike, trample, or shadow: until end of turn, target creature with the chosen ability loses it and another target creature gains it"];
}
