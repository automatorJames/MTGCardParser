namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dusk Mangler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dusk Mangler</exampledoc>
[TokenizationOrder(-1)]
public class DuskManglerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as an additional cost to cast this spell, sacrifice a creature, discard a card, or pay 4 life"];
}
