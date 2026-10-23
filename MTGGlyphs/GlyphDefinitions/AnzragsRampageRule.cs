namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Anzrag's Rampage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Anzrag's Rampage</exampledoc>
/// <examplecapture>destroy all artifacts you don't control, then exile the top x cards of your library, where x is the number of artifacts that were put into graveyards from the battlefield this turn</examplecapture>
[TokenizationOrder(-1)]
public class AnzragsRampageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all artifacts you don't control, then exile the top x cards of your library, where x is the number of artifacts that were put into graveyards from the battlefield this turn"];
}
