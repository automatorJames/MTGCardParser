namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Summoning Trap, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Summoning Trap</exampledoc>
/// <examplecapture>if a creature spell you cast this turn was countered by a spell or ability an opponent controlled, you may pay {0} rather than pay this spell's mana cost</examplecapture>
[TokenizationOrder(-1)]
public class SummoningTrapRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature spell you cast this turn was countered by a spell or ability an opponent controlled, you may pay {0} rather than pay this spell's mana cost"];
}
