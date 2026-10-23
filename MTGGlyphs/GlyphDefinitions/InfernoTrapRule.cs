namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Inferno Trap, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Inferno Trap</exampledoc>
[TokenizationOrder(-1)]
public class InfernoTrapRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you've been dealt damage by two or more creatures this turn, you may pay {r} rather than pay this spell's mana cost"];
}
