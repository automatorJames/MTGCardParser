namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Runescale Stormbrood // Chilling Screech, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Runescale Stormbrood // Chilling Screech</exampledoc>
[TokenizationOrder(-1)]
public class RunescaleStormbroodCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast a noncreature spell or a dragon spell, this creature gets +2/+0 until end of turn"];
}
