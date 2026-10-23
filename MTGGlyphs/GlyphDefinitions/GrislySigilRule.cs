namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Grisly Sigil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grisly Sigil</exampledoc>
[TokenizationOrder(-1)]
public class GrislySigilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it was dealt noncombat damage this turn,", Nib.This, "deals 3 damage to it and you gain 3 life"];
}
