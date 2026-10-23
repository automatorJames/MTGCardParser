namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fear of Burning Alive, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fear of Burning Alive</exampledoc>
/// <examplecapture>if there are four or more card types among cards in your graveyard, this creature deals that amount of damage to target creature that player controls</examplecapture>
[Dependent]
public class FearOfBurningAliveEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if there are four or more card types among cards in your graveyard, this creature deals that amount of damage to target creature that player controls"];
}
