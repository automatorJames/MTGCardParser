namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dimir Charm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dimir Charm</exampledoc>
[TokenizationOrder(-1)]
public class DimirCharmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put one back and the rest into that player's graveyard"];
}
