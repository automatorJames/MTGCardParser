namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Niko, Light of Hope, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Niko, Light of Hope</exampledoc>
/// <examplecapture>they're enchantments with "{2}, sacrifice this token: scry 1, then draw a card</examplecapture>
[Dependent]
public class NikoLightOfHopeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they're enchantments with \"{2}, sacrifice this token: scry 1, then draw a card"];
}
