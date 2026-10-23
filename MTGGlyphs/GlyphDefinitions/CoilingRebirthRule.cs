namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Coiling Rebirth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coiling Rebirth</exampledoc>
[TokenizationOrder(-1)]
public class CoilingRebirthRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if the gift was promised and that creature isn't legendary, create a token that's a copy of that creature, except it's 1/1"];
}
