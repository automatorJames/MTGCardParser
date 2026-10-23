namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Infamous Cruelclaw, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Infamous Cruelclaw</exampledoc>
[TokenizationOrder(-1)]
public class TheInfamousCruelclawRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast that card by discarding a card rather than paying its mana cost"];
}
