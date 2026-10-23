namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Eye of the Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eye of the Storm</exampledoc>
[TokenizationOrder(-1)]
public class EyeOfTheStormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each copy, the player may cast the copy without paying its mana cost"];
}
