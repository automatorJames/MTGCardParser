namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Arsenal Thresher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Arsenal Thresher</exampledoc>
[TokenizationOrder(-1)]
public class ArsenalThresherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as this creature enters, you may reveal any number of other artifact cards from your hand"];
}
