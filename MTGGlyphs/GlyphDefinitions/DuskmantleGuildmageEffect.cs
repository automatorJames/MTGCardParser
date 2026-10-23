namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Duskmantle Guildmage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Duskmantle Guildmage</exampledoc>
[Dependent]
public class DuskmantleGuildmageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a card is put into an opponent's graveyard from anywhere this turn, that player loses 1 life"];
}
