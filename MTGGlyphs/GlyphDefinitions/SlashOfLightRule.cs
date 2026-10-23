namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Slash of Light, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Slash of Light</exampledoc>
[TokenizationOrder(-1)]
public class SlashOfLightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage equal to the number of creatures you control plus the number of equipment you control to target creature"];
}
