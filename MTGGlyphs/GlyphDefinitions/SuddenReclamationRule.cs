namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sudden Reclamation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sudden Reclamation</exampledoc>
[TokenizationOrder(-1)]
public class SuddenReclamationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mill four cards, then return a creature card and a land card from your graveyard to your hand"];
}
