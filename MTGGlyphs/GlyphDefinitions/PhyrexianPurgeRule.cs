namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Phyrexian Purge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Purge</exampledoc>
[TokenizationOrder(-1)]
public class PhyrexianPurgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this spell costs 3 life more to cast for each target"];
}
