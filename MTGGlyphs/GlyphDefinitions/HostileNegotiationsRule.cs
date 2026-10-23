namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hostile Negotiations, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hostile Negotiations</exampledoc>
/// <examplecapture>exile the top three cards of your library in a face-down pile, then exile the top three cards of your library in another face-down pile</examplecapture>
[TokenizationOrder(-1)]
public class HostileNegotiationsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile the top three cards of your library in a face-down pile, then exile the top three cards of your library in another face-down pile"];
}
