namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Warden of the Wall, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Warden of the Wall</exampledoc>
[TokenizationOrder(-1)]
public class WardenOfTheWallRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during turns other than yours, this artifact is a 2/3 gargoyle artifact creature with flying"];
}
