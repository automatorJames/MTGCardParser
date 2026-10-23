namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Goliath Hatchery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goliath Hatchery</exampledoc>
/// <examplecapture>if an opponent has three or more poison counters, choose a creature you control, then draw cards equal to its total toxic value</examplecapture>
[Dependent]
public class GoliathHatcheryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an opponent has three or more poison counters, choose a creature you control, then draw cards equal to its total toxic value"];
}
