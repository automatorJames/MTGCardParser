namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Search for Survivors, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Search for Survivors</exampledoc>
[TokenizationOrder(-1)]
public class SearchForSurvivorsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reorder your graveyard at random"];
}
