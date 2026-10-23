namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Living Death, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Living Death</exampledoc>
/// <examplecapture>each player exiles all creature cards from their graveyard, then sacrifices all creatures they control, then puts all cards they exiled this way onto the battlefield</examplecapture>
[TokenizationOrder(-1)]
public class LivingDeathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player exiles all creature cards from their graveyard, then sacrifices all creatures they control, then puts all cards they exiled this way onto the battlefield"];
}
