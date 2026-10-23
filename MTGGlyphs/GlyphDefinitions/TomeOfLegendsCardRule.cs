namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tome of Legends, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tome of Legends</exampledoc>
[TokenizationOrder(-1)]
public class TomeOfLegendsCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever your commander enters or attacks, put a page counter on this artifact"];
}
