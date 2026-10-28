namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Siren's Call, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Siren's Call</exampledoc>
[TokenizationOrder(-1)]
public class SirensCallRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures the active player controls attack this turn if able"];
}
