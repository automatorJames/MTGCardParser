namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hoarder's Overflow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hoarder's Overflow</exampledoc>
[TokenizationOrder(-1)]
public class HoardersOverflowCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this enchantment enters and whenever you expend 4, put a stash counter on it"];
}
