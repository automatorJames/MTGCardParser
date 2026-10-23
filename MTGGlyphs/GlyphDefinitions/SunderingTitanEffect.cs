namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sundering Titan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sundering Titan</exampledoc>
[Dependent]
public class SunderingTitanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a land of each basic land type, then destroy those lands"];
}
