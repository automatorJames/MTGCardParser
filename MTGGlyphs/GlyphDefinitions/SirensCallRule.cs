namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Siren's Call, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Siren's Call</exampledoc>
[TokenizationOrder(-1)]
public class SirensCallRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cast this spell only during an opponent's turn, before attackers are declared"];
}
