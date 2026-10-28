namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fear of Burning Alive, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fear of Burning Alive</exampledoc>
[TokenizationOrder(-1)]
public class FearOfBurningAliveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["delirium — whenever a source you control deals noncombat damage to an opponent, if there are four or more card types among cards in your graveyard, this creature deals that amount of damage to target creature that player controls"];
}
