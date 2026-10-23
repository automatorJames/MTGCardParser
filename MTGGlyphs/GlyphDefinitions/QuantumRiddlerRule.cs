namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Quantum Riddler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quantum Riddler</exampledoc>
/// <examplecapture>as long as you have one or fewer cards in hand, if you would draw one or more cards, you draw that many cards plus one instead</examplecapture>
[TokenizationOrder(-1)]
public class QuantumRiddlerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as you have one or fewer cards in hand, if you would draw one or more cards, you draw that many cards plus one instead"];
}
