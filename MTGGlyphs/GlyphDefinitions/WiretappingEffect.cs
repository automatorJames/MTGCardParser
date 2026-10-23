namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wiretapping, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wiretapping</exampledoc>
[Dependent]
public class WiretappingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}
