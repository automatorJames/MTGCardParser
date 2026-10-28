namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Neglected Heirloom // Ashmouth Blade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Neglected Heirloom // Ashmouth Blade</exampledoc>
[TokenizationOrder(-1)]
public class NeglectedHeirloomRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when equipped creature transforms, transform this equipment"];
}
