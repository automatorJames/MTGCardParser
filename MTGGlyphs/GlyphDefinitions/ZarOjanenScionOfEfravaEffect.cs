namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Zar Ojanen, Scion of Efrava, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zar Ojanen, Scion of Efrava</exampledoc>
/// <examplecapture>put a +1/+1 counter on each creature you control with toughness less than the number of basic land types among lands you control</examplecapture>
[Dependent]
public class ZarOjanenScionOfEfravaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on each creature you control with toughness less than the number of basic land types among lands you control"];
}
