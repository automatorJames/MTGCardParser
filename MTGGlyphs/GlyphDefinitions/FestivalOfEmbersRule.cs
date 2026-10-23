namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Festival of Embers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Festival of Embers</exampledoc>
[TokenizationOrder(-1)]
public class FestivalOfEmbersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your turn, you may cast instant and sorcery spells from your graveyard by paying 1 life in addition to their other costs"];
}
