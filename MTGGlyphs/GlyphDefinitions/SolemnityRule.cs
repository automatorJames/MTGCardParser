namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Solemnity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Solemnity</exampledoc>
[TokenizationOrder(-1)]
public class SolemnityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counters can't be put on artifacts, creatures, enchantments, or lands"];
}
