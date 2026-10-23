namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Last Agni Kai, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Last Agni Kai</exampledoc>
[TokenizationOrder(-1)]
public class TheLastAgniKaiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the creature the opponent controls is dealt excess damage this way, add that much {r}"];
}
