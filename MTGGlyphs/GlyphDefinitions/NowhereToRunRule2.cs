namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nowhere to Run, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nowhere to Run</exampledoc>
[TokenizationOrder(-1)]
public class NowhereToRunRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ward abilities of those creatures don't trigger"];
}
