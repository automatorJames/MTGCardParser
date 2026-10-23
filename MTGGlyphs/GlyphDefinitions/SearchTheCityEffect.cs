namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Search the City, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Search the City</exampledoc>
/// <examplecapture>you may put one of those cards with that name into its owner's hand</examplecapture>
[Dependent]
public class SearchTheCityEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put one of those cards with that name into its owner's hand"];
}
