namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Linvala, Shield of Sea Gate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Linvala, Shield of Sea Gate</exampledoc>
[Dependent]
public class LinvalaShieldOfSeaGateCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose hexproof or indestructible"];
}
