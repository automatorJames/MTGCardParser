namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ashes of the Fallen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashes of the Fallen</exampledoc>
[TokenizationOrder(-1)]
public class AshesOfTheFallenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature card in your graveyard has the chosen creature type in addition to its other types"];
}
