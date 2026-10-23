namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nowhere to Run, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nowhere to Run</exampledoc>
[TokenizationOrder(-1)]
public class NowhereToRunRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures your opponents control can be the targets of spells and abilities as though they didn't have hexproof"];
}
