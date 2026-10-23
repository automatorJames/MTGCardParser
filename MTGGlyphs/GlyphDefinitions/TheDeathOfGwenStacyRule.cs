namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Death of Gwen Stacy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Death of Gwen Stacy</exampledoc>
[TokenizationOrder(-1)]
public class TheDeathOfGwenStacyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["iii — exile any number of target players' graveyards"];
}
