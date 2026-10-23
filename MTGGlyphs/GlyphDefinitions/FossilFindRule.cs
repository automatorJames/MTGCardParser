namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fossil Find, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fossil Find</exampledoc>
[TokenizationOrder(-1)]
public class FossilFindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return a card at random from your graveyard to your hand, then reorder your graveyard as you choose"];
}
