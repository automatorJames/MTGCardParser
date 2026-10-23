namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ruric Thar, Magecrusher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ruric Thar, Magecrusher</exampledoc>
[TokenizationOrder(-1)]
public class RuricTharMagecrusherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ruric thar has hexproof as long as they haven't dealt combat damage yet"];
}
