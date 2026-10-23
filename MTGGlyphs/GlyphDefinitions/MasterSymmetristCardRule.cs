namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Master Symmetrist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master Symmetrist</exampledoc>
[TokenizationOrder(-1)]
public class MasterSymmetristCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control with power equal to its toughness attacks, it gains trample until end of turn"];
}
