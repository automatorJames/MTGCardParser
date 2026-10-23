namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Stangg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stangg</exampledoc>
[TokenizationOrder(-1)]
public class StanggCard3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice", Nib.This, "when that token leaves the battlefield"];
}
