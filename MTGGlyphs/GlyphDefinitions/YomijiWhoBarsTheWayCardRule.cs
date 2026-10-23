namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Yomiji, Who Bars the Way, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yomiji, Who Bars the Way</exampledoc>
[TokenizationOrder(-1)]
public class YomijiWhoBarsTheWayCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a legendary permanent other than yomiji is put into a graveyard from the battlefield, return that card to its owner's hand"];
}
