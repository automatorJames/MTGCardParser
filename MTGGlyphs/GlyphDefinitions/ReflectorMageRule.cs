namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reflector Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reflector Mage</exampledoc>
[TokenizationOrder(-1)]
public class ReflectorMageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature's owner can't cast spells with the same name as that creature until your next turn"];
}
