namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jhoira's Timebug, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jhoira's Timebug</exampledoc>
/// <examplecapture>if that permanent or card has a time counter on it, you may remove a time counter from it or put another time counter on it</examplecapture>
[TokenizationOrder(-1)]
public class JhoirasTimebugRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that permanent or card has a time counter on it, you may remove a time counter from it or put another time counter on it"];
}
