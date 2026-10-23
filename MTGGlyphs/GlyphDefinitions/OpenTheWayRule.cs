namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Open the Way, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Open the Way</exampledoc>
[TokenizationOrder(-1)]
public class OpenTheWayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x can't be greater than the number of players in the game"];
}
