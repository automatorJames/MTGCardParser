namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Call to Arms, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Call to Arms</exampledoc>
/// <examplecapture>sacrifice this enchantment</examplecapture>
[Dependent]
public class CallToArms2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice this enchantment"];
}
