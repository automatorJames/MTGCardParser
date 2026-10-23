namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lullmage's Domination, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lullmage's Domination</exampledoc>
[TokenizationOrder(-1)]
public class LullmagesDominationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this spell costs {3} less to cast if it targets a creature whose controller has eight or more cards in their graveyard"];
}
