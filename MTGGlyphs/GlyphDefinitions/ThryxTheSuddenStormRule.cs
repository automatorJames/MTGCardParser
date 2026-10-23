namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thryx, the Sudden Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thryx, the Sudden Storm</exampledoc>
[TokenizationOrder(-1)]
public class ThryxTheSuddenStormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spells you cast with mana value 5 or greater cost {1} less to cast and can't be countered"];
}
