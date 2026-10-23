namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Visions, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Visions</exampledoc>
[TokenizationOrder(-1)]
public class VisionsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may then have that player shuffle that library"];
}
