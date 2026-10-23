namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Haunting Echoes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haunting Echoes</exampledoc>
[TokenizationOrder(-1)]
public class HauntingEchoesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all cards from target player's graveyard other than basic land cards"];
}
