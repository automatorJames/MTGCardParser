namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gideon Blackblade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gideon Blackblade</exampledoc>
[TokenizationOrder(-1)]
public class GideonBlackbladeRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage that would be dealt to", Nib.This, "during your turn"];
}
