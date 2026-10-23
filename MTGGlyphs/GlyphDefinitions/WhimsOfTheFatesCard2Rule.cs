namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Whims of the Fates, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Whims of the Fates</exampledoc>
[TokenizationOrder(-1)]
public class WhimsOfTheFatesCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["starting with you, each player separates all permanents they control into three piles"];
}
