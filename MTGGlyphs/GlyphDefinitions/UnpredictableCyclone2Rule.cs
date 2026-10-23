namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Unpredictable Cyclone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unpredictable Cyclone</exampledoc>
[TokenizationOrder(-1)]
public class UnpredictableCyclone2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then put the exiled cards that weren't cast this way on the bottom of your library in a random order"];
}
