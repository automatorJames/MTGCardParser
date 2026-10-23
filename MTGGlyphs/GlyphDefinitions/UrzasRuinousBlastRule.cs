namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Urza's Ruinous Blast, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Urza's Ruinous Blast</exampledoc>
[TokenizationOrder(-1)]
public class UrzasRuinousBlastRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all nonland permanents that aren't legendary"];
}
