namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Muldrotha, the Gravetide, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Muldrotha, the Gravetide</exampledoc>
[TokenizationOrder(-1)]
public class MuldrothaTheGravetideRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during each of your turns, you may play a land and cast a permanent spell of each permanent type from your graveyard"];
}
