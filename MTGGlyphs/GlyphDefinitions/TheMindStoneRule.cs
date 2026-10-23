namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Mind Stone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Mind Stone</exampledoc>
/// <examplecapture>∞ — at the beginning of your end step, exile up to one other target nonland permanent you control, then return that card to the battlefield under its owner's control</examplecapture>
[TokenizationOrder(-1)]
public class TheMindStoneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["∞ — at the beginning of your end step, exile up to one other target nonland permanent you control, then return that card to the battlefield under its owner's control"];
}
