namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Prized Amalgam, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Prized Amalgam</exampledoc>
/// <examplecapture>if it entered from your graveyard or you cast it from your graveyard, return this card from your graveyard to the battlefield tapped at the beginning of the next end step</examplecapture>
[Dependent]
public class PrizedAmalgamEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it entered from your graveyard or you cast it from your graveyard, return this card from your graveyard to the battlefield tapped at the beginning of the next end step"];
}
