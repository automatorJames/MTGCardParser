namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Master's Councillors, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master's Councillors</exampledoc>
[TokenizationOrder(-1)]
public class MastersCouncillorsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +2/+0 for each graveyard with seven or more cards in it"];
}
