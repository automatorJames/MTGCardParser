namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glittering Lynx, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glittering Lynx</exampledoc>
[TokenizationOrder(-1)]
public class GlitteringLynxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{2}: until end of turn, this creature loses \"prevent all damage that would be dealt to this creature"];
}
