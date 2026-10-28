namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glittering Lion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glittering Lion</exampledoc>
[TokenizationOrder(-1)]
public class GlitteringLionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{3}: until end of turn, this creature loses \"prevent all damage that would be dealt to this creature"];
}
