namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Raiding Party, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raiding Party</exampledoc>
[TokenizationOrder(-1)]
public class RaidingPartyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then destroy all plains that weren't chosen this way by any player"];
}
