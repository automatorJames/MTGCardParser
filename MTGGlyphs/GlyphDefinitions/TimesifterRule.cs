namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Timesifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Timesifter</exampledoc>
[TokenizationOrder(-1)]
public class TimesifterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two or more players' cards are tied for greatest, the tied players repeat this process until the tie is broken"];
}
