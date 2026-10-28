namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Grisly Sigil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grisly Sigil</exampledoc>
[TokenizationOrder(-1)]
public class GrislySigilRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise,", Nib.This, "deals 1 damage to it and you gain 1 life"];
}
