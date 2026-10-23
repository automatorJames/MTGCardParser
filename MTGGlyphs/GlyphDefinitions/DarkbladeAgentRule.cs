namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Darkblade Agent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Darkblade Agent</exampledoc>
/// <examplecapture>as long as you've surveilled this turn, this creature has deathtouch and "whenever this creature deals combat damage to a player, you draw a card</examplecapture>
[TokenizationOrder(-1)]
public class DarkbladeAgentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as you've surveilled this turn, this creature has deathtouch and \"whenever this creature deals combat damage to a player, you draw a card"];
}
