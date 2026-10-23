namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mistmeadow Skulk, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mistmeadow Skulk</exampledoc>
[TokenizationOrder(-1)]
public class MistmeadowSkulkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lifelink, protection from mana value 3 or greater"];
}
