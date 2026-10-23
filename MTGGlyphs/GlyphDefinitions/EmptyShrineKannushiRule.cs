namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Empty-Shrine Kannushi, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Empty-Shrine Kannushi</exampledoc>
[TokenizationOrder(-1)]
public class EmptyShrineKannushiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has protection from each color among permanents you control"];
}
