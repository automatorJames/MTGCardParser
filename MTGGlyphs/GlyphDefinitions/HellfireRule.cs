namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hellfire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hellfire</exampledoc>
[TokenizationOrder(-1)]
public class HellfireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals x plus 3 damage to you, where x is the number of creatures that died this way"];
}
