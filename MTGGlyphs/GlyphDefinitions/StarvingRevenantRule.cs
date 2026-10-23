namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Starving Revenant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Starving Revenant</exampledoc>
[TokenizationOrder(-1)]
public class StarvingRevenantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then for each card you put on top of your library, you draw a card and you lose 3 life"];
}
