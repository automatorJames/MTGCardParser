namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tunnel Vision, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tunnel Vision</exampledoc>
/// <examplecapture>if it is, that player puts the rest of the revealed cards into their graveyard and puts the card with the chosen name on top of their library</examplecapture>
[TokenizationOrder(-1)]
public class TunnelVisionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it is, that player puts the rest of the revealed cards into their graveyard and puts the card with the chosen name on top of their library"];
}
