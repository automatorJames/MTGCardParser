namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Xanathar, Guild Kingpin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Xanathar, Guild Kingpin</exampledoc>
/// <examplecapture>until end of turn, that player can't cast spells, you may look at the top card of their library any time, you may play the top card of their library, and you may spend mana as though it were mana of any color to cast spells this way</examplecapture>
[TokenizationOrder(-1)]
public class XanatharGuildKingpinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, that player can't cast spells, you may look at the top card of their library any time, you may play the top card of their library, and you may spend mana as though it were mana of any color to cast spells this way"];
}
