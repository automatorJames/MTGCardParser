namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Blinding Beam, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blinding Beam</exampledoc>
[TokenizationOrder(-1)]
public class BlindingBeamRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• creatures don't untap during target player's next untap step"];
}
