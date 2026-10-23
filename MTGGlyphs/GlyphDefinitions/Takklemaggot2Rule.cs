namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Takklemaggot, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Takklemaggot</exampledoc>
[TokenizationOrder(-1)]
public class Takklemaggot2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they don't, return this card to the battlefield under your control as a non-aura enchantment"];
}
