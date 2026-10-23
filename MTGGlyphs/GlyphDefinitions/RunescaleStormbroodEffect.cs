namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Runescale Stormbrood // Chilling Screech, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Runescale Stormbrood // Chilling Screech</exampledoc>
[Dependent]
public class RunescaleStormbroodEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +2/+0 until end of turn"];
}
