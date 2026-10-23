namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thalia, Heretic Cathar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thalia, Heretic Cathar</exampledoc>
[TokenizationOrder(-1)]
public class ThaliaHereticCatharRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures and nonbasic lands your opponents control enter tapped"];
}
