namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dreadhound, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dreadhound</exampledoc>
[TokenizationOrder(-1)]
public class DreadhoundRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature dies or a creature card is put into a graveyard from a library, each opponent loses 1 life"];
}
