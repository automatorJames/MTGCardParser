namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Bears of Littjara, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Bears of Littjara</exampledoc>
[TokenizationOrder(-1)]
public class TheBearsOfLittjaraRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature with power 4 or greater you control deals damage equal to its power to that permanent"];
}
