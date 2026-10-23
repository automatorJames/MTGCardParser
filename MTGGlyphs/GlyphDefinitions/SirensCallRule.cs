namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Siren's Call, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Siren's Call</exampledoc>
[TokenizationOrder(-1)]
public class SirensCallRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ignore this effect for each creature the player didn't control continuously since the beginning of the turn"];
}
