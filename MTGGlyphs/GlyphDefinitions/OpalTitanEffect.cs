namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Opal Titan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Opal Titan</exampledoc>
/// <examplecapture>if this permanent is an enchantment, it becomes a 4/4 giant creature with protection from each of that spell's colors</examplecapture>
[Dependent]
public class OpalTitanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this permanent is an enchantment, it becomes a 4/4 giant creature with protection from each of that spell's colors"];
}
