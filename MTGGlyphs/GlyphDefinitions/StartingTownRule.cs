namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Starting Town, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Starting Town</exampledoc>
[TokenizationOrder(-1)]
public class StartingTownRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this land enters tapped unless it's your first, second, or third turn of the game"];
}
