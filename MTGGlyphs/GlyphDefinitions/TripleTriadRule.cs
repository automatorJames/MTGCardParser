namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Triple Triad, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Triple Triad</exampledoc>
/// <examplecapture>until end of turn, you may play the card you own exiled this way and each other card exiled this way with lesser mana value than it without paying their mana costs</examplecapture>
[TokenizationOrder(-1)]
public class TripleTriadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, you may play the card you own exiled this way and each other card exiled this way with lesser mana value than it without paying their mana costs"];
}
