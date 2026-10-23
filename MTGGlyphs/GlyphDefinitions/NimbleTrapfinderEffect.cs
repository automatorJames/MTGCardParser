namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nimble Trapfinder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nimble Trapfinder</exampledoc>
/// <examplecapture>if you have a full party, creatures you control gain "whenever this creature deals combat damage to a player, draw a card" until end of turn</examplecapture>
[Dependent]
public class NimbleTrapfinderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you have a full party, creatures you control gain \"whenever this creature deals combat damage to a player, draw a card\" until end of turn"];
}
