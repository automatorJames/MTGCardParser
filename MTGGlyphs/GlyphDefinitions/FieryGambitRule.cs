namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fiery Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fiery Gambit</exampledoc>
[TokenizationOrder(-1)]
public class FieryGambitRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win three or more flips, draw nine cards and untap all lands you control"];
}
