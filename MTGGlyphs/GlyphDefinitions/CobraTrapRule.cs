namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cobra Trap, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cobra Trap</exampledoc>
/// <examplecapture>if a noncreature permanent under your control was destroyed this turn by a spell or ability an opponent controlled, you may pay {g} rather than pay this spell's mana cost</examplecapture>
[TokenizationOrder(-1)]
public class CobraTrapRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a noncreature permanent under your control was destroyed this turn by a spell or ability an opponent controlled, you may pay {g} rather than pay this spell's mana cost"];
}
