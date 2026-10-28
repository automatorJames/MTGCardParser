namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reverse the Sands, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reverse the Sands</exampledoc>
[TokenizationOrder(-1)]
public class ReverseTheSandsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["redistribute any number of players' life totals"];
}
