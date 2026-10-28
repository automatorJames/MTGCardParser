namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spider-Verse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Verse</exampledoc>
[TokenizationOrder(-1)]
public class SpiderVerseRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the copy is a permanent spell, it gains haste"];
}
