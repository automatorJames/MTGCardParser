namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bark of Doran, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bark of Doran</exampledoc>
[TokenizationOrder(-1)]
public class BarkOfDoranRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as equipped creature's toughness is greater than its power, it assigns combat damage equal to its toughness rather than its power"];
}
