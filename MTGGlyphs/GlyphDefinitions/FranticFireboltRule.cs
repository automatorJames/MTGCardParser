namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Frantic Firebolt, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Frantic Firebolt</exampledoc>
/// <examplecapture>{this} deals x damage to target creature, where x is 2 plus the number of cards in your graveyard that are instant cards, sorcery cards, and/or have an adventure</examplecapture>
[TokenizationOrder(-1)]
public class FranticFireboltRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals x damage to target creature, where x is 2 plus the number of cards in your graveyard that are instant cards, sorcery cards, and/or have an adventure"];
}
