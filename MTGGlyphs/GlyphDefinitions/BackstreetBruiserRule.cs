namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Backstreet Bruiser, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Backstreet Bruiser</exampledoc>
[TokenizationOrder(-1)]
public class BackstreetBruiserRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as there are two or more counters among creatures you control, this creature can attack as though it didn't have defender"];
}
