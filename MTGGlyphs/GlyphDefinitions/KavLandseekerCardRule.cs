namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kav Landseeker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kav Landseeker</exampledoc>
[TokenizationOrder(-1)]
public class KavLandseekerCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["at the beginning of the end step on your next turn, sacrifice that token"];
}
