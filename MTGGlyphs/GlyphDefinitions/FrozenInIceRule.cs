namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Frozen in Ice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Frozen in Ice</exampledoc>
[TokenizationOrder(-1)]
public class FrozenInIceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature loses all abilities and can't become untapped"];
}
