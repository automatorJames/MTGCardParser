namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Stangg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stangg</exampledoc>
[TokenizationOrder(-1)]
public class StanggRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile that token when", Nib.This, "leaves the battlefield"];
}
