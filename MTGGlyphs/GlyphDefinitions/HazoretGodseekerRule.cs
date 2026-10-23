namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hazoret, Godseeker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hazoret, Godseeker</exampledoc>
[TokenizationOrder(-1)]
public class HazoretGodseekerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["hazoret can't attack or block unless you have max speed"];
}
