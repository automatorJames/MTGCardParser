namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Holistic Wisdom, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Holistic Wisdom</exampledoc>
/// <examplecapture>return target card from your graveyard to your hand if it shares a card type with the card exiled this way</examplecapture>
[Dependent]
public class HolisticWisdomEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target card from your graveyard to your hand if it shares a card type with the card exiled this way"];
}
