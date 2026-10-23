namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of World War Hulk, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>World War Hulk</exampledoc>
[TokenizationOrder(-1)]
public class WorldWarHulkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["i — the next red or green creature spell you cast this turn can be cast without paying its mana cost"];
}
