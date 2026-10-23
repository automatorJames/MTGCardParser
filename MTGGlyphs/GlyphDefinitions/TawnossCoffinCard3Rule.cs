namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tawnos's Coffin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tawnos's Coffin</exampledoc>
[TokenizationOrder(-1)]
public class TawnossCoffinCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["note the number and kind of counters that were on that creature"];
}
