namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kaya, Spirits' Justice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaya, Spirits' Justice</exampledoc>
/// <examplecapture>you may choose a creature card from among them</examplecapture>
[Dependent]
public class KayaSpiritsJusticeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may choose a creature card from among them"];
}
