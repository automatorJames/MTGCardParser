namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rootwater Shaman, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rootwater Shaman</exampledoc>
[TokenizationOrder(-1)]
public class RootwaterShamanRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast aura spells with enchant creature as though they had flash"];
}
